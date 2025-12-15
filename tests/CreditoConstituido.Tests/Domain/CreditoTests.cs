using CreditoConstituido.Domain.Entities;
using CreditoConstituido.Domain.Errors;

namespace CreditoConstituido.Tests.Domain;

public sealed class CreditoTests
{
    public static IEnumerable<object[]> BodiesSemelhantesAoEmail()
    {
        yield return new object[]
        {
            "123456",
            "7891011",
            "2024-02-25",
            1500.75m,
            "ISSQN",
            true,
            5.0m,
            30000.00m,
            5000.00m,
            25000.00m
        };

        yield return new object[]
        {
            "789012",
            "7891011",
            "2024-02-26",
            1200.50m,
            "ISSQN",
            false,
            4.5m,
            25000.00m,
            4000.00m,
            21000.00m
        };

        yield return new object[]
        {
            "654321",
            "1122334",
            "2024-01-15",
            800.50m,
            "Outros",
            true,
            3.5m,
            20000.00m,
            3000.00m,
            17000.00m
        };
    }

    [Theory]
    [MemberData(nameof(BodiesSemelhantesAoEmail))]
    public void Constructor_DeveCriarCredito_QuandoDadosValidos_ComBodySemelhanteAoEmail(
        string numeroCredito,
        string numeroNfse,
        string dataConstituicao,
        decimal valorIssqn,
        string tipoCredito,
        bool simplesNacional,
        decimal aliquota,
        decimal valorFaturado,
        decimal valorDeducao,
        decimal baseCalculo)
    {
        var credito = new Credito(
            numeroCredito: numeroCredito,
            numeroNfse: numeroNfse,
            dataConstituicao: DateOnly.Parse(dataConstituicao),
            valorIssqn: valorIssqn,
            tipoCredito: tipoCredito,
            simplesNacional: simplesNacional,
            aliquota: aliquota,
            valorFaturado: valorFaturado,
            valorDeducao: valorDeducao,
            baseCalculo: baseCalculo
        );

        credito.NumeroCredito.Should().Be(numeroCredito);
        credito.NumeroNfse.Should().Be(numeroNfse);
        credito.DataConstituicao.Should().Be(DateOnly.Parse(dataConstituicao));
        credito.ValorIssqn.Should().Be(valorIssqn);
        credito.TipoCredito.Should().Be(tipoCredito);
        credito.SimplesNacional.Should().Be(simplesNacional);
        credito.Aliquota.Should().Be(aliquota);
        credito.ValorFaturado.Should().Be(valorFaturado);
        credito.ValorDeducao.Should().Be(valorDeducao);
        credito.BaseCalculo.Should().Be(baseCalculo);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_DeveLancarDomainException_QuandoNumeroCreditoInvalido(string? numeroCredito)
    {
        var act = () => new Credito(
            numeroCredito: numeroCredito!,
            numeroNfse: "7891011",
            dataConstituicao: DateOnly.Parse("2024-02-25"),
            valorIssqn: 1500.75m,
            tipoCredito: "ISSQN",
            simplesNacional: true,
            aliquota: 5.0m,
            valorFaturado: 30000.00m,
            valorDeducao: 5000.00m,
            baseCalculo: 25000.00m
        );

        act.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_DeveLancarDomainException_QuandoNumeroNfseInvalido(string? numeroNfse)
    {
        var act = () => new Credito(
            numeroCredito: "123456",
            numeroNfse: numeroNfse!,
            dataConstituicao: DateOnly.Parse("2024-02-25"),
            valorIssqn: 1500.75m,
            tipoCredito: "ISSQN",
            simplesNacional: true,
            aliquota: 5.0m,
            valorFaturado: 30000.00m,
            valorDeducao: 5000.00m,
            baseCalculo: 25000.00m
        );

        act.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_DeveLancarDomainException_QuandoTipoCreditoInvalido(string? tipoCredito)
    {
        var act = () => new Credito(
            numeroCredito: "123456",
            numeroNfse: "7891011",
            dataConstituicao: DateOnly.Parse("2024-02-25"),
            valorIssqn: 1500.75m,
            tipoCredito: tipoCredito!,
            simplesNacional: true,
            aliquota: 5.0m,
            valorFaturado: 30000.00m,
            valorDeducao: 5000.00m,
            baseCalculo: 25000.00m
        );

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Constructor_DeveDarTrim_EmCamposString_ComValoresDoEmail()
    {
        var credito = new Credito(
            numeroCredito: "  123456  ",
            numeroNfse: "  7891011  ",
            dataConstituicao: DateOnly.Parse("2024-02-25"),
            valorIssqn: 1500.75m,
            tipoCredito: "  ISSQN  ",
            simplesNacional: true,
            aliquota: 5.0m,
            valorFaturado: 30000.00m,
            valorDeducao: 5000.00m,
            baseCalculo: 25000.00m
        );

        credito.NumeroCredito.Should().Be("123456");
        credito.NumeroNfse.Should().Be("7891011");
        credito.TipoCredito.Should().Be("ISSQN");
        credito.SimplesNacional.Should().BeTrue();
    }
}
