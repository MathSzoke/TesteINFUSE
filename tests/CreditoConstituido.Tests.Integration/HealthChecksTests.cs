using System.Net;
using FluentAssertions;

namespace CreditoConstituido.Tests.Integration;

public sealed class HealthChecksTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;

    public HealthChecksTests(ApiFactory factory) => this._factory = factory;

    [Fact]
    public async Task Self_e_Ready_DevemRetornar_200()
    {
        using var client = this._factory.CreateApiClient();

        var self = await client.GetAsync("/self");
        var ready = await client.GetAsync("/ready");

        self.StatusCode.Should().Be(HttpStatusCode.OK);
        ready.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}