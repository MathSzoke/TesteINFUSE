using CreditoConstituido.Domain.Entities;

namespace CreditoConstituido.Application.Interfaces;

public interface ICreditoRepository
{
    Task<List<Credito>> GetByNumeroNfseAsync(string numeroNfse, CancellationToken ct);
    Task<Credito?> GetByNumeroCreditoAsync(string numeroCredito, CancellationToken ct);
    Task<bool> ExistsByNumeroCreditoAsync(string numeroCredito, CancellationToken ct);
    Task AddAsync(Credito credito, CancellationToken ct);
}