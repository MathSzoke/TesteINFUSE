using Confluent.Kafka;
using CreditoConstituido.Application.Dtos;
using CreditoConstituido.Application.Interfaces;
using CreditoConstituido.Domain.Entities;
using CreditoConstituido.Domain.ValueObjects;
using CreditoConstituido.Infrastructure.Messaging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace CreditoConstituido.Api.HostedServices;

public sealed class CreditoIntegrationWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly KafkaOptions _options;
    private readonly ILogger<CreditoIntegrationWorker> _logger;

    public CreditoIntegrationWorker(
        IServiceScopeFactory scopeFactory,
        IOptions<KafkaOptions> options,
        ILogger<CreditoIntegrationWorker> logger)
    {
        this._scopeFactory = scopeFactory;
        this._options = options.Value;
        this._logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = this._options.BootstrapServers,
            GroupId = this._options.ConsumerGroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();

        this._logger.LogInformation(
            "[Worker] starting | topic={Topic} group={Group} bootstrap={Bootstrap}",
            this._options.IntegrationTopicName,
            this._options.ConsumerGroupId,
            this._options.BootstrapServers);

        consumer.Subscribe(this._options.IntegrationTopicName);

        var tick = 0L;

        while (!stoppingToken.IsCancellationRequested)
        {
            tick++;

            try
            {
                var cr = consumer.Consume(TimeSpan.FromMilliseconds(50));

                if (cr is null)
                {
                    this._logger.LogDebug("[Worker] tick={Tick} | no message | sleeping=500ms", tick);
                    await Task.Delay(500, stoppingToken);
                    continue;
                }

                this._logger.LogInformation(
                    "[Worker] tick={Tick} | consumed | topic={Topic} partition={Partition} offset={Offset}",
                    tick,
                    cr.Topic,
                    cr.Partition.Value,
                    cr.Offset.Value);

                var dto = JsonSerializer.Deserialize<IntegrarCreditoRequestItemDto>(
                    cr.Message.Value,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (dto is null)
                {
                    this._logger.LogWarning(
                        "[Worker] tick={Tick} | invalid payload (deserialize null) | committing offset={Offset}",
                        tick,
                        cr.Offset.Value);

                    consumer.Commit(cr);
                    continue;
                }

                var numeroCredito = (dto.NumeroCredito ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(numeroCredito))
                {
                    this._logger.LogWarning(
                        "[Worker] tick={Tick} | skipping (numeroCredito empty) | committing offset={Offset}",
                        tick,
                        cr.Offset.Value);

                    consumer.Commit(cr);
                    continue;
                }

                using var scope = _scopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<ICreditoRepository>();
                var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var exists = await repo.ExistsByNumeroCreditoAsync(numeroCredito, stoppingToken);

                if (exists)
                {
                    this._logger.LogInformation(
                        "[Worker] tick={Tick} | duplicate ignored | numeroCredito={NumeroCredito} | committing offset={Offset}",
                        tick,
                        numeroCredito,
                        cr.Offset.Value);

                    consumer.Commit(cr);
                    continue;
                }

                var entity = new Credito(
                    numeroCredito: numeroCredito,
                    numeroNfse: (dto.NumeroNfse ?? string.Empty).Trim(),
                    dataConstituicao: DateOnly.FromDateTime(dto.DataConstituicao),
                    valorIssqn: dto.ValorIssqn,
                    tipoCredito: (dto.TipoCredito ?? string.Empty).Trim(),
                    simplesNacional: SimplesNacionalParser.Parse(dto.SimplesNacional),
                    aliquota: dto.Aliquota,
                    valorFaturado: dto.ValorFaturado,
                    valorDeducao: dto.ValorDeducao,
                    baseCalculo: dto.BaseCalculo
                );

                await repo.AddAsync(entity, stoppingToken);
                await uow.SaveChangesAsync(stoppingToken);

                this._logger.LogInformation(
                    "[Worker] tick={Tick} | inserted | numeroCredito={NumeroCredito} numeroNfse={NumeroNfse} data={Data} | committing offset={Offset}",
                    tick,
                    entity.NumeroCredito,
                    entity.NumeroNfse,
                    entity.DataConstituicao.ToString("yyyy-MM-dd"),
                    cr.Offset.Value);

                consumer.Commit(cr);
            }
            catch (OperationCanceledException)
            {
                this._logger.LogInformation("[Worker] stopping (cancellation requested)");
                break;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "[Worker] error tick={Tick} | sleeping=500ms", tick);
                await Task.Delay(500, stoppingToken);
            }
        }

        try
        {
            consumer.Close();
        }
        catch (Exception ex)
        {
            this._logger.LogWarning(ex, "[Worker] consumer close failed");
        }

        this._logger.LogInformation("[Worker] stopped");
    }
}
