using CreditoConstituido.Application.Interfaces;
using CreditoConstituido.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CreditoConstituido.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICreditoQueryService, CreditoQueryService>();
        return services;
    }
}