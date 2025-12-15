using System.Text.Json;
using Confluent.Kafka;
using CreditoConstituido.Application.Dtos;
using CreditoConstituido.Application.Interfaces;
using Microsoft.Extensions.Options;

namespace CreditoConstituido.Infrastructure.Messaging;

public sealed class KafkaCreditoIntegrationPublisher : ICreditoIntegrationPublisher
{
    private readonly IProducer<string, string> _producer;
    private readonly KafkaOptions _options;

    public KafkaCreditoIntegrationPublisher(IProducer<string, string> producer, IOptions<KafkaOptions> options)
    {
        this._producer = producer;
        this._options = options.Value;
    }

    public async Task PublishAsync(IntegrarCreditoRequestItemDto credito, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(credito, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        var msg = new Message<string, string>
        {
            Key = (credito.NumeroCredito ?? string.Empty).Trim(),
            Value = json
        };

        var dr = await this._producer.ProduceAsync(this._options.IntegrationTopicName, msg, ct);
        if (dr.Status is PersistenceStatus.NotPersisted)
            throw new InvalidOperationException("Mensagem não persistida no Kafka");
    }
}