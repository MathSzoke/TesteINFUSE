using CreditoConstituido.Domain.Entities;

namespace CreditoConstituido.Tests.Domain;

public sealed class CreditoInvalidNumericValuesTests
{
    [Theory]
    [InlineData(-1)]
    [InlineData(-0.01)]
    public void Constructor_DeveAceitarValoresNegativos_PorNaoHaverRegraNoDominio(decimal valor)
    {
        var credito = new Credito(
            numeroCredito: "123456",
            numeroNfse: "7891011",
            dataConstituicao: DateOnly.Parse("2024-02-25"),
            valorIssqn: valor,
            tipoCredito: "ISSQN",
            simplesNacional: true,
            aliquota: valor,
            valorFaturado: valor,
            valorDeducao: valor,
            baseCalculo: valor
        );

        credito.Should().NotBeNull();
    }
}