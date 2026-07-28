using Inventory.Api.Features;
using Inventory.Api.GrpcServices;
using Inventory.Api.Consumers;
using MassTransit;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();

    // Keep local gRPC independent from the machine's HTTPS developer certificate.
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenLocalhost(5141, listenOptions =>
        {
            listenOptions.Protocols = HttpProtocols.Http1;
        });

        options.ListenLocalhost(5142, listenOptions =>
        {
            listenOptions.Protocols = HttpProtocols.Http2;
        });
    });
}
else
{
    // Docker uses separate clear-text HTTP/1.1 and HTTP/2 ports.
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(8004, listenOptions =>
        {
            listenOptions.Protocols = HttpProtocols.Http1;
        });

        options.ListenAnyIP(8007, listenOptions =>
        {
            listenOptions.Protocols = HttpProtocols.Http2;
        });
    });
}

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
