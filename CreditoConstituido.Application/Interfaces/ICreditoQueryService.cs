using CreditoConstituido.Application.Dtos;

namespace CreditoConstituido.Application.Interfaces;

public interface ICreditoQueryService
{
    Task<List<CreditoDto>> GetByNumeroNfseAsync(string numeroNfse, CancellationToken ct);
    Task<CreditoDto?> GetByNumeroCreditoAsync(string numeroCredito, CancellationToken ct);
}