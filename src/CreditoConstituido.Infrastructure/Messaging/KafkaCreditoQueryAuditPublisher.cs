using System.Text.Json;
using Confluent.Kafka;
using CreditoConstituido.Application.Interfaces;
using Microsoft.Extensions.Options;

namespace CreditoConstituido.Infrastructure.Messaging;

public sealed class KafkaCreditoQueryAuditPublisher : ICreditoQueryAuditPublisher
{
    private readonly IProducer<string, string> _producer;
    private readonly KafkaOptions _options;

    public KafkaCreditoQueryAuditPublisher(IProducer<string, string> producer, IOptions<KafkaOptions> options)
    {
        this._producer = producer;
        this._options = options.Value;
    }

    public async Task PublishConsultaAsync(string tipoConsulta, string valor, DateTimeOffset timestamp, CancellationToken ct)
    {
        var payload = new
        {
            tipoConsulta,
            valor,
            timestamp = timestamp.ToString("O")
        };

        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        var msg = new Message<string, string>
        {
            Key = $"{tipoConsulta}:{valor}",
            Value = json
        };

        var dr = await this._producer.ProduceAsync(_options.AuditTopicName, msg, ct);
        if (dr.Status is PersistenceStatus.NotPersisted)
            throw new InvalidOperationException("Mensagem de auditoria não persistida no Kafka");
    }
}