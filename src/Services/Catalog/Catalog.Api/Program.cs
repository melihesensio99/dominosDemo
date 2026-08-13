using Catalog.Api.Features;
using Catalog.Api.Infrastructure.Configuration;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddCatalogModule(builder.Configuration);

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        var host = builder.Configuration["RabbitMq:Host"]
            ?? throw new InvalidOperationException("RabbitMq:Host is missing.");
        var username = builder.Configuration["RabbitMq:Username"]
            ?? throw new InvalidOperationException("RabbitMq:Username is missing.");
        var password = builder.Configuration["RabbitMq:Password"]
            ?? throw new InvalidOperationException("RabbitMq:Password is missing.");

        cfg.Host(host, "/", h =>
        {
            h.Username(username);
            h.Password(password);
        });
    });
});

var app = builder.Build();

app.UseGlobalExceptionHandler();

app.MapCatalogEndpoints();

await app.Services.InitializeCatalogDatabaseAsync();

app.Run();
