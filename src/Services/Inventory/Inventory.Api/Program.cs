using Inventory.Api.Features;
using Inventory.Api.GrpcServices;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

InventoryModule.ConfigureServices(builder.Services, builder.Configuration);
builder.Services.AddGrpc();

builder.Services.AddMassTransit(x =>
{
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
    });
});

var app = builder.Build();

app.UseGlobalExceptionHandler();

app.MapGrpcService<InventoryStockGrpcService>();

app.MapGet("/health/live", () => Results.Ok(new
{
    service = Environment.GetEnvironmentVariable("SERVICE_NAME") ?? "inventory",
    status = "ok",
}));

app.MapGet("/health/ready", async (InventoryDbContext dbContext, CancellationToken cancellationToken) =>
{
    var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);

    return canConnect
        ? Results.Ok(new
        {
            service = Environment.GetEnvironmentVariable("SERVICE_NAME") ?? "inventory",
            status = "ready",
        })
        : Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
});

app.MapInventoryEndpoints();

await app.Services.InitializeInventoryDatabaseAsync();

app.Run();
