namespace CreditoConstituido.Tests.Domain;

public sealed class CreditoParsingTests
{
    [Theory]
    [InlineData("Sim", true)]
    [InlineData("sim", true)]
    [InlineData("SIM", true)]
    [InlineData("  Sim  ", true)]
    [InlineData("Não", false)]
    [InlineData("não", false)]
    [InlineData("NAO", false)]
    [InlineData("nao", false)]
    [InlineData("  Não  ", false)]
    public void ParseSimNao_DeveConverterCorretamente(string input, bool esperado)
    {
        var result = ParseSimNao(input);

        result.Should().Be(esperado);
    }

    [Theory]
    [InlineData("Talvez")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("S")]
    [InlineData("N")]
    [InlineData("Yes")]
    public void ParseSimNao_DeveLancarExcecao_QuandoValorInvalido(string input)
    {
        var act = () => ParseSimNao(input);

        act.Should().Throw<ArgumentException>();
    }

    private static bool ParseSimNao(string value)
    {
        var v = value.Trim().ToLowerInvariant();
        return v switch
        {
            "sim" => true,
            "não" => false,
            "nao" => false,
            _ => throw new ArgumentException($"Valor inválido para SimplesNacional: {value}")
        };
    }
}