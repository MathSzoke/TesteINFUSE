using CreditoConstituido.Api.HostedServices;
using CreditoConstituido.Application;
using CreditoConstituido.Infrastructure;
using CreditoConstituido.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHostedService<CreditoIntegrationWorker>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CreditoDbContext>();
    await db.Database.MigrateAsync();
}

app.MapControllers();

app.Run();

namespace CreditoConstituido.Api
{
    public class Program { }
}