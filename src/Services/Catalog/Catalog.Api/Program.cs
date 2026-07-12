using Catalog.Api.Features;

var builder = WebApplication.CreateBuilder(args);

CatalogModule.ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

app.UseGlobalExceptionHandler();

app.MapGet("/health/live", () => Results.Ok(new
{
    service = Environment.GetEnvironmentVariable("SERVICE_NAME") ?? "catalog",
    status = "ok",
}));

app.MapGet("/health/ready", async (CatalogDbContext dbContext, CancellationToken cancellationToken) =>
{
    var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);

    return canConnect
        ? Results.Ok(new
        {
            service = Environment.GetEnvironmentVariable("SERVICE_NAME") ?? "catalog",
            status = "ready",
        })
        : Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
});

app.MapCatalogEndpoints();

await app.Services.InitializeCatalogDatabaseAsync();

app.Run();
