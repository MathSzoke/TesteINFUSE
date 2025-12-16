using System.Text.Json;
using Confluent.Kafka;
using CreditoConstituido.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CreditoConstituido.Infrastructure.Messaging;

public sealed class KafkaCreditoQueryAuditPublisher : ICreditoQueryAuditPublisher
{
    private readonly IProducer<string, string> _producer;
    private readonly KafkaOptions _options;
    private readonly ILogger<KafkaCreditoQueryAuditPublisher> _logger;

    public KafkaCreditoQueryAuditPublisher(
        IProducer<string, string> producer,
        IOptions<KafkaOptions> options,
        ILogger<KafkaCreditoQueryAuditPublisher> logger)
    {
        this._producer = producer;
        this._options = options.Value;
        this._logger = logger;
    }

    public async Task PublishConsultaAsync(string tipoConsulta, string valor, DateTimeOffset timestamp, CancellationToken ct)
    {
        var payload = new
        {
            tipoConsulta,
            valor,
            timestamp = timestamp.ToString("O")
        };

        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var msg = new Message<string, string>
        {
            Key = $"{tipoConsulta}:{valor}",
            Value = json
        };

        this._logger.LogInformation("[Audit] publishing | topic={Topic} | key={Key} | value={Value}", this._options.AuditTopicName, msg.Key, msg.Value);

        var dr = await this._producer.ProduceAsync(this._options.AuditTopicName, msg, ct);

        this._logger.LogInformation("[Audit] published | topic={Topic} | partition={Partition} | offset={Offset} | status={Status}",
            this._options.AuditTopicName,
            dr.Partition.Value,
            dr.Offset.Value,
            dr.Status);

        if (dr.Status is PersistenceStatus.NotPersisted)
        {
            this._logger.LogError("[Audit] NOT persisted | topic={Topic} | key={Key}", this._options.AuditTopicName, msg.Key);
            throw new InvalidOperationException("Mensagem de auditoria não persistida no Kafka");
        }
    }
}
