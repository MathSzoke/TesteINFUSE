using Confluent.Kafka;
using CreditoConstituido.Application.Interfaces;
using CreditoConstituido.Infrastructure.Data;
using CreditoConstituido.Infrastructure.Messaging;
using CreditoConstituido.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CreditoConstituido.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<KafkaOptions>(configuration.GetSection("Kafka"));

        services.AddDbContext<CreditoDbContext>(opt =>
        {
            var cs = configuration.GetConnectionString("Default");
            opt.UseNpgsql(cs!);
        });

        services.AddSingleton(sp =>
        {
            var opts = sp.GetRequiredService<IOptions<KafkaOptions>>().Value;
            return new ProducerBuilder<string, string>(new ProducerConfig
            {
                BootstrapServers = opts.BootstrapServers,
                Acks = Acks.All,
                EnableIdempotence = true
            }).Build();
        });

        services.AddScoped<ICreditoRepository, CreditoRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<ICreditoIntegrationPublisher, KafkaCreditoIntegrationPublisher>();
        services.AddScoped<ICreditoQueryAuditPublisher, KafkaCreditoQueryAuditPublisher>();

        return services;
    }
}