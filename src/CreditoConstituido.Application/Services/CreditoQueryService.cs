using CreditoConstituido.Application.Dtos;
using CreditoConstituido.Application.Interfaces;
using CreditoConstituido.Domain.Entities;

namespace CreditoConstituido.Application.Services;

public sealed class CreditoQueryService : ICreditoQueryService
{
    private readonly ICreditoRepository _repo;
    private readonly ICreditoQueryAuditPublisher _audit;

    public CreditoQueryService(ICreditoRepository repo, ICreditoQueryAuditPublisher audit)
    {
        this._repo = repo;
        this._audit = audit;
    }

    public async Task<List<CreditoDto>> GetByNumeroNfseAsync(string numeroNfse, CancellationToken ct)
    {
        var list = await this._repo.GetByNumeroNfseAsync(numeroNfse, ct);
        await this._audit.PublishConsultaAsync("ByNfse", numeroNfse, DateTimeOffset.UtcNow, ct);
        return list.Select(Map).ToList();
    }

    public async Task<CreditoDto?> GetByNumeroCreditoAsync(string numeroCredito, CancellationToken ct)
    {
        var entity = await this._repo.GetByNumeroCreditoAsync(numeroCredito, ct);
        await this._audit.PublishConsultaAsync("ByNumeroCredito", numeroCredito, DateTimeOffset.UtcNow, ct);
        return entity is null ? null : Map(entity);
    }

    private static CreditoDto Map(Credito e)
    {
        return new CreditoDto
        {
            NumeroCredito = e.NumeroCredito,
            NumeroNfse = e.NumeroNfse,
            DataConstituicao = e.DataConstituicao.ToString("yyyy-MM-dd"),
            ValorIssqn = e.ValorIssqn,
            TipoCredito = e.TipoCredito,
            SimplesNacional = e.SimplesNacional ? "Sim" : "Não",
            Aliquota = e.Aliquota,
            ValorFaturado = e.ValorFaturado,
            ValorDeducao = e.ValorDeducao,
            BaseCalculo = e.BaseCalculo
        };
    }
}