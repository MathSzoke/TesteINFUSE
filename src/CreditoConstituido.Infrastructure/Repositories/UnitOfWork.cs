using CreditoConstituido.Application.Interfaces;
using CreditoConstituido.Infrastructure.Data;

namespace CreditoConstituido.Infrastructure.Repositories;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly CreditoDbContext _db;

    public UnitOfWork(CreditoDbContext db) =>
        this._db = db;

    public Task<int> SaveChangesAsync(CancellationToken ct) => 
        this._db
            .SaveChangesAsync(ct);
}