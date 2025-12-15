using CreditoConstituido.Application.Interfaces;
using CreditoConstituido.Domain.Entities;
using CreditoConstituido.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CreditoConstituido.Infrastructure.Repositories;

public sealed class CreditoRepository : ICreditoRepository
{
    private readonly CreditoDbContext _db;

    public CreditoRepository(CreditoDbContext db) => this._db = db;

    public Task<List<Credito>> GetByNumeroNfseAsync(string numeroNfse, CancellationToken ct) => 
        this._db.Creditos.AsNoTracking()
            .Where(x => x.NumeroNfse == numeroNfse)
            .OrderBy(x => x.DataConstituicao)
            .ToListAsync(ct);

    public Task<Credito?> GetByNumeroCreditoAsync(string numeroCredito, CancellationToken ct) =>
        this._db.Creditos.AsNoTracking()
            .FirstOrDefaultAsync(x => x.NumeroCredito == numeroCredito, ct);

    public Task<bool> ExistsByNumeroCreditoAsync(string numeroCredito, CancellationToken ct)=>
        this._db.Creditos.AsNoTracking()
            .AnyAsync(x => x.NumeroCredito == numeroCredito, ct);

    public Task AddAsync(Credito credito, CancellationToken ct) =>
        this._db.Creditos
            .AddAsync(credito, ct)
            .AsTask();
}