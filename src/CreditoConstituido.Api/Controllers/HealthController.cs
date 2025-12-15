using Confluent.Kafka;
using CreditoConstituido.Infrastructure.Data;
using CreditoConstituido.Infrastructure.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CreditoConstituido.Api.Controllers;

[ApiController]
public sealed class HealthController : ControllerBase
{
    private readonly CreditoDbContext _db;
    private readonly KafkaOptions _options;

    public HealthController(CreditoDbContext db, IOptions<KafkaOptions> options)
    {
        this._db = db;
        this._options = options.Value;
    }

    [HttpGet("/self")]
    public IActionResult Self()
    {
        return this.Ok(new { status = "ok" });
    }

    [HttpGet("/ready")]
    public async Task<IActionResult> Ready(CancellationToken ct)
    {
        var dbOk = await this._db.Database.CanConnectAsync(ct);

        var kafkaOk = false;
        try
        {
            using var admin = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = _options.BootstrapServers }).Build();
            var md = admin.GetMetadata(TimeSpan.FromSeconds(2));
            kafkaOk = md is not null && md.Brokers is not null && md.Brokers.Count > 0;
        }
        catch
        {
            kafkaOk = false;
        }

        if (dbOk && kafkaOk) return this.Ok(new { status = "ready" });
        return this.StatusCode(503, new { status = "not-ready", dbOk, kafkaOk });
    }
}