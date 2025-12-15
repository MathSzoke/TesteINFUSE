namespace CreditoConstituido.Infrastructure.Messaging;

public sealed class KafkaOptions
{
    public string BootstrapServers { get; set; } = string.Empty;
    public string IntegrationTopicName { get; set; } = "integrar-credito-constituido-entry";
    public string AuditTopicName { get; set; } = "credito-consulta-audit";
    public string ConsumerGroupId { get; set; } = "credito-constituido-worker";
}