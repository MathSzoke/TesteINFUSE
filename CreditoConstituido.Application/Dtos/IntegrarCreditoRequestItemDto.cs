namespace CreditoConstituido.Application.Dtos;

public sealed class IntegrarCreditoRequestItemDto
{
    public string NumeroCredito { get; set; } = string.Empty;
    public string NumeroNfse { get; set; } = string.Empty;
    public DateOnly DataConstituicao { get; set; }
    public decimal ValorIssqn { get; set; }
    public string TipoCredito { get; set; } = string.Empty;
    public string SimplesNacional { get; set; } = string.Empty;
    public decimal Aliquota { get; set; }
    public decimal ValorFaturado { get; set; }
    public decimal ValorDeducao { get; set; }
    public decimal BaseCalculo { get; set; }
}