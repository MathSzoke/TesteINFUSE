using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace CreditoConstituido.Tests.Integration;

public sealed class CreditoDuplicadoTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;

    public CreditoDuplicadoTests(ApiFactory factory) => this._factory = factory;

    [Fact]
    public async Task Post_ComNumeroCreditoDuplicado_NaoDeveInserirDuplicado()
    {
        using var client = this._factory.CreateApiClient();

        var payload = new[]
        {
            new
            {
                numeroCredito = "999999",
                numeroNfse = "NFSE-DUP",
                dataConstituicao = "2024-03-01",
                valorIssqn = 1000m,
                tipoCredito = "ISSQN",
                simplesNacional = "Sim",
                aliquota = 5m,
                valorFaturado = 20000m,
                valorDeducao = 2000m,
                baseCalculo = 18000m
            },
            new
            {
                numeroCredito = "999999",
                numeroNfse = "NFSE-DUP",
                dataConstituicao = "2024-03-01",
                valorIssqn = 1000m,
                tipoCredito = "ISSQN",
                simplesNacional = "Sim",
                aliquota = 5m,
                valorFaturado = 20000m,
                valorDeducao = 2000m,
                baseCalculo = 18000m
            }
        };

        var post = await client.PostAsJsonAsync("/api/creditos/integrar-credito-constituido", payload);
        post.StatusCode.Should().Be(HttpStatusCode.Accepted);

        var itens = await PollAsync(async () =>
        {
            var get = await client.GetAsync("/api/creditos/NFSE-DUP");
            if (get.StatusCode != HttpStatusCode.OK) return null;

            var body = await get.Content.ReadFromJsonAsync<List<CreditoView>>();
            if (body is null || body.Count == 0) return null;

            return body;
        }, 15000, 300);

        itens.Should().NotBeNull();
        itens!.Count.Should().Be(1);
        itens[0].NumeroCredito.Should().Be("999999");
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
    }
}