using Confluent.Kafka;
using CreditoConstituido.Application.Dtos;
using CreditoConstituido.Application.Interfaces;
using CreditoConstituido.Domain.Entities;
using CreditoConstituido.Domain.ValueObjects;
using CreditoConstituido.Infrastructure.Messaging;
using Microsoft.Extensions.Options;
using System;
using System.Text.Json;

namespace CreditoConstituido.Api.HostedServices;

public sealed class CreditoIntegrationWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly KafkaOptions _options;

    public CreditoIntegrationWorker(IServiceScopeFactory scopeFactory, IOptions<KafkaOptions> options)
    {
        this._scopeFactory = scopeFactory;
        this._options = options.Value;
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
        consumer.Subscribe(this._options.IntegrationTopicName);

        while (!stoppingToken.IsCancellationRequested)
        {
            ConsumeResult<string, string>? cr = null;

            try
            {
                cr = consumer.Consume(TimeSpan.FromMilliseconds(50));
                if (cr is null)
                {
                    await Task.Delay(500, stoppingToken);
                    continue;
                }

                var dto = JsonSerializer.Deserialize<IntegrarCreditoRequestItemDto>(
                    cr.Message.Value,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (dto is null)
                {
                    consumer.Commit(cr);
                    continue;
                }

                var numeroCredito = (dto.NumeroCredito ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(numeroCredito))
                {
                    consumer.Commit(cr);
                    continue;
                }

                using var scope = this._scopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<ICreditoRepository>();
                var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var exists = await repo.ExistsByNumeroCreditoAsync(numeroCredito, stoppingToken);

                if (!exists)
                {
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
                }

                consumer.Commit(cr);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch
            {
                await Task.Delay(500, stoppingToken);
            }
        }

        consumer.Close();
    }
}