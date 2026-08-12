using Inventory.Api.Features;
using Inventory.Api.GrpcServices;
using Inventory.Api.Infrastructure.Configuration;
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

builder.Services.AddInventoryModule(builder.Configuration);

var app = builder.Build();

app.UseGlobalExceptionHandler();

app.MapGrpcService<InventoryStockGrpcService>();

app.MapInventoryEndpoints();

await app.Services.InitializeInventoryDatabaseAsync();

app.Run();
