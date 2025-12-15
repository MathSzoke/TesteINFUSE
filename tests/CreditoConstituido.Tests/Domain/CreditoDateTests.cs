using CreditoConstituido.Domain.Entities;
using FluentAssertions;

namespace CreditoConstituido.Tests.Domain;

public sealed class CreditoDateTests
{
    [Fact]
    public void Constructor_DeveAceitarDataMinima()
    {
        var credito = new Credito(
            numeroCredito: "123456",
            numeroNfse: "7891011",
            dataConstituicao: DateOnly.MinValue,
            valorIssqn: 100,
            tipoCredito: "ISSQN",
            simplesNacional: true,
            aliquota: 5,
            valorFaturado: 1000,
            valorDeducao: 0,
            baseCalculo: 1000
        );

        credito.DataConstituicao.Should().Be(DateOnly.MinValue);
    }

    [Fact]
    public void Constructor_DeveAceitarDataFutura_PorNaoHaverRegra()
    {
        var futura = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(5));

        var credito = new Credito(
            numeroCredito: "123456",
            numeroNfse: "7891011",
            dataConstituicao: futura,
            valorIssqn: 100,
            tipoCredito: "ISSQN",
            simplesNacional: true,
            aliquota: 5,
            valorFaturado: 1000,
            valorDeducao: 0,
            baseCalculo: 1000
        );

        credito.DataConstituicao.Should().Be(futura);
    }
}