using Inventory.Api.Features;
using Inventory.Api.GrpcServices;
using Inventory.Api.Consumers;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

InventoryModule.ConfigureServices(builder.Services, builder.Configuration);
builder.Services.AddGrpc();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ProductCreatedConsumer>();
    x.SetKebabCaseEndpointNameFormatter();

    x.UsingRabbitMq((context, cfg) =>
    {
        var host = builder.Configuration["RabbitMq:Host"] ?? "localhost";
        var username = builder.Configuration["RabbitMq:Username"] ?? "guest";
        var password = builder.Configuration["RabbitMq:Password"] ?? "guest";

        cfg.Host(host, "/", h =>
        {
            h.Username(username);
            h.Password(password);
        });

        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.UseGlobalExceptionHandler();

app.MapGrpcService<InventoryStockGrpcService>();

app.MapInventoryEndpoints();

await app.Services.InitializeInventoryDatabaseAsync();

app.Run();
