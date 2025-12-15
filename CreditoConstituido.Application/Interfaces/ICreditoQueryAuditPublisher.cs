namespace CreditoConstituido.Application.Interfaces;

public interface ICreditoQueryAuditPublisher
{
    Task PublishConsultaAsync(string tipoConsulta, string valor, DateTimeOffset timestamp, CancellationToken ct);
}