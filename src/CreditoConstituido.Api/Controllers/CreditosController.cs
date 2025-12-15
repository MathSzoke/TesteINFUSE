using CreditoConstituido.Application.Dtos;
using CreditoConstituido.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CreditoConstituido.Api.Controllers;

[ApiController]
[Route("api/creditos")]
public sealed class CreditosController : ControllerBase
{
    private readonly ICreditoIntegrationPublisher _publisher;
    private readonly ICreditoQueryService _query;

    public CreditosController(ICreditoIntegrationPublisher publisher, ICreditoQueryService query)
    {
        this._publisher = publisher;
        this._query = query;
    }

    [HttpPost("integrar-credito-constituido")]
    public async Task<IActionResult> IntegrarCreditoConstituido([FromBody] List<IntegrarCreditoRequestItemDto> creditos, CancellationToken ct)
    {
        if (creditos is null || creditos.Count == 0)
            return this.BadRequest(new { success = false, error = "Lista de créditos é obrigatória" });

        var distinct = creditos
            .Where(x => !string.IsNullOrWhiteSpace(x.NumeroCredito))
            .GroupBy(x => x.NumeroCredito.Trim())
            .Select(g => g.First())
            .ToList();

        foreach (var c in distinct)
            await this._publisher.PublishAsync(c, ct);

        return this.Accepted(new { success = true });
    }

    [HttpGet("{numeroNfse}")]
    public async Task<IActionResult> GetByNumeroNfse([FromRoute] string numeroNfse, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(numeroNfse))
            return this.BadRequest(new { success = false, error = "numeroNfse é obrigatório" });

        var list = await this._query.GetByNumeroNfseAsync(numeroNfse.Trim(), ct);
        return this.Ok(list);
    }

    [HttpGet("credito/{numeroCredito}")]
    public async Task<IActionResult> GetByNumeroCredito([FromRoute] string numeroCredito, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(numeroCredito))
            return this.BadRequest(new { success = false, error = "numeroCredito é obrigatório" });

        var item = await this._query.GetByNumeroCreditoAsync(numeroCredito.Trim(), ct);
        if (item is null) return this.NotFound();
        return this.Ok(item);
    }
}