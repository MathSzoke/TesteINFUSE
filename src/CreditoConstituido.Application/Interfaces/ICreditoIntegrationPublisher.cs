using CreditoConstituido.Application.Dtos;

namespace CreditoConstituido.Application.Interfaces;

public interface ICreditoIntegrationPublisher
{
    Task PublishAsync(IntegrarCreditoRequestItemDto credito, CancellationToken ct);
}