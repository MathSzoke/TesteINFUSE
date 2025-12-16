using Testcontainers.Kafka;
using Testcontainers.PostgreSql;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using CreditoConstituido.Api;
using DotNet.Testcontainers.Builders;

namespace CreditoConstituido.Tests.Integration;

public sealed class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres;
    private readonly KafkaContainer _kafka;
    private bool _started;

    public string ConnectionString { get; private set; } = string.Empty;
    public string KafkaBootstrapServers { get; private set; } = string.Empty;

    public ApiFactory()
    {
        this._postgres = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("credito_db")
            .WithUsername("credito")
            .WithPassword("credito")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
            .Build();

        this._kafka = new KafkaBuilder()
            .WithImage("confluentinc/cp-kafka:7.6.1")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(9092))
            .Build();
    }

    public async Task InitializeAsync()
    {
        if (this._started) return;

        await this._postgres.StartAsync();
        await this._kafka.StartAsync();

        ConnectionString = this._postgres.GetConnectionString();
        KafkaBootstrapServers = this._kafka.GetBootstrapAddress();

        this._started = true;
    }

    public new async Task DisposeAsync()
    {
        await this._kafka.DisposeAsync();
        await this._postgres.DisposeAsync();
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((ctx, cfg) =>
        {
            Environment.SetEnvironmentVariable("ConnectionStrings__Default", ConnectionString);
            Environment.SetEnvironmentVariable("Kafka__BootstrapServers", KafkaBootstrapServers);
        });

        return base.CreateHost(builder);
    }

    public HttpClient CreateApiClient() =>
        this.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
}
