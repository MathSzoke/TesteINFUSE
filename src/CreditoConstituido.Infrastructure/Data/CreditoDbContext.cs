using CreditoConstituido.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CreditoConstituido.Infrastructure.Data;

public sealed class CreditoDbContext : DbContext
{
    public DbSet<Credito> Creditos => this.Set<Credito>();

    public CreditoDbContext(DbContextOptions<CreditoDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CreditoDbContext).Assembly);
    }
}