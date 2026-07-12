using Basket.Api.Features;

var builder = WebApplication.CreateBuilder(args);

BasketModule.ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

app.UseGlobalExceptionHandler();

app.MapGet("/health/live", () => Results.Ok(new
{
    service = Environment.GetEnvironmentVariable("SERVICE_NAME") ?? "basket",
    status = "ok",
}));

app.MapGet("/health/ready", async (IInventoryStockClient stockClient, CancellationToken cancellationToken) =>
{
    _ = stockClient;

    var ready = await stockClient.IsReadyAsync(cancellationToken);

    return ready
        ? Results.Ok(new
        {
            service = Environment.GetEnvironmentVariable("SERVICE_NAME") ?? "basket",
            status = "ready",
        })
        : Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
});

app.MapBasketEndpoints();

app.Run();
