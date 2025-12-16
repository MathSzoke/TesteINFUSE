using System.Net;
using FluentAssertions;

namespace CreditoConstituido.Tests.Integration;

public sealed class CreditoInexistenteTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;

    public CreditoInexistenteTests(ApiFactory factory) => this._factory = factory;

    [Fact]
    public async Task Get_PorNumeroCredito_Inexistente_DeveRetornar_404()
    {
        using var client = this._factory.CreateApiClient();

        var response = await client.GetAsync("/api/creditos/credito/NAO-EXISTE");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}