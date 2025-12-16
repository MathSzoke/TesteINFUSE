using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace CreditoConstituido.Tests.Integration;

public sealed class NfseInexistenteTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;

    public NfseInexistenteTests(ApiFactory factory) => this._factory = factory;

    [Fact]
    public async Task Get_PorNfse_Inexistente_DeveRetornar_ListaVazia()
    {
        using var client = this._factory.CreateApiClient();

        var response = await client.GetAsync("/api/creditos/NFSE-INEXISTENTE");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<List<object>>();
        body.Should().NotBeNull();
        body!.Should().BeEmpty();
    }
}