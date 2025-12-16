using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace CreditoConstituido.Tests.Integration;

public sealed class CreditosFlowTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;

    public CreditosFlowTests(ApiFactory factory) => this._factory = factory;

    [Fact]
    public async Task Post_PublicaNoKafka_WorkerConsome_InsereNoPostgres_E_GetPorNfseRetorna()
    {
        using var client = this._factory.CreateApiClient();

        var payload = new[]
        {
            new
            {
                numeroCredito = "123456",
                numeroNfse = "7891011",
                dataConstituicao = "2024-02-25",
                valorIssqn = 1500.75m,
                tipoCredito = "ISSQN",
                simplesNacional = "Sim",
                aliquota = 5.0m,
                valorFaturado = 30000.00m,
                valorDeducao = 5000.00m,
                baseCalculo = 25000.00m
            },
            new
            {
                numeroCredito = "789012",
                numeroNfse = "7891011",
                dataConstituicao = "2024-02-26",
                valorIssqn = 1200.50m,
                tipoCredito = "ISSQN",
                simplesNacional = "Não",
                aliquota = 4.5m,
                valorFaturado = 25000.00m,
                valorDeducao = 4000.00m,
                baseCalculo = 21000.00m
            }
        };

        var post = await client.PostAsJsonAsync("/api/creditos/integrar-credito-constituido", payload);
        post.StatusCode.Should().Be(HttpStatusCode.Accepted);

        var postBody = await post.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        postBody.Should().NotBeNull();
        postBody!.ContainsKey("success").Should().BeTrue();
        postBody["success"].ToString()!.ToLowerInvariant().Should().Be("true");

        var itens = await PollAsync(async () =>
        {
            var get = await client.GetAsync("/api/creditos/7891011");
            if (get.StatusCode != HttpStatusCode.OK) return null;

            var body = await get.Content.ReadFromJsonAsync<List<CreditoView>>();
            if (body is null || body.Count < 2) return null;

            return body;
        }, timeoutMs: 15000, delayMs: 300);

        itens.Should().NotBeNull();
        itens!.Count.Should().BeGreaterOrEqualTo(2);

        itens.Select(x => x.NumeroCredito).Should().Contain(new[] { "123456", "789012" });
        itens.All(x => x.NumeroNfse == "7891011").Should().BeTrue();
        itens.Select(x => x.DataConstituicao).Should().Contain(new[] { "2024-02-25", "2024-02-26" });
        itens.Select(x => x.SimplesNacional).Should().OnlyContain(x => x == "Sim" || x == "Não");
    }

    [Fact]
    public async Task GetPorNumeroCredito_RetornaUmItem()
    {
        using var client = this._factory.CreateApiClient();

        var payload = new[]
        {
            new
            {
                numeroCredito = "654321",
                numeroNfse = "1122334",
                dataConstituicao = "2024-01-15",
                valorIssqn = 800.50m,
                tipoCredito = "Outros",
                simplesNacional = "Sim",
                aliquota = 3.5m,
                valorFaturado = 20000.00m,
                valorDeducao = 3000.00m,
                baseCalculo = 17000.00m
            }
        };

        var post = await client.PostAsJsonAsync("/api/creditos/integrar-credito-constituido", payload);
        post.StatusCode.Should().Be(HttpStatusCode.Accepted);

        var item = await PollAsync(async () =>
        {
            var get = await client.GetAsync("/api/creditos/credito/654321");
            if (get.StatusCode == HttpStatusCode.NotFound) return null;
            if (get.StatusCode != HttpStatusCode.OK) return null;

            return await get.Content.ReadFromJsonAsync<CreditoView>();
        }, timeoutMs: 15000, delayMs: 300);

        item.Should().NotBeNull();
        item!.NumeroCredito.Should().Be("654321");
        item.NumeroNfse.Should().Be("1122334");
        item.DataConstituicao.Should().Be("2024-01-15");
        item.SimplesNacional.Should().Be("Sim");
        item.TipoCredito.Should().Be("Outros");
    }

    private static async Task<T?> PollAsync<T>(Func<Task<T?>> action, int timeoutMs, int delayMs) where T : class
    {
        var start = Environment.TickCount64;

        while (Environment.TickCount64 - start < timeoutMs)
        {
            var result = await action();
            if (result is not null) return result;
            await Task.Delay(delayMs);
        }

        return null;
    }

    private sealed class CreditoView
    {
        public string NumeroCredito { get; set; } = string.Empty;
        public string NumeroNfse { get; set; } = string.Empty;
        public string DataConstituicao { get; set; } = string.Empty;
        public decimal ValorIssqn { get; set; }
        public string TipoCredito { get; set; } = string.Empty;
        public string SimplesNacional { get; set; } = string.Empty;
        public decimal Aliquota { get; set; }
        public decimal ValorFaturado { get; set; }
        public decimal ValorDeducao { get; set; }
        public decimal BaseCalculo { get; set; }
    }
}