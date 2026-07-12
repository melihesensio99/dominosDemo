using Auth.Api.Features;
using BuildingBlocks.Web;

var builder = WebApplication.CreateBuilder(args);

AuthModule.ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

app.UseGlobalExceptionHandler();

app.MapGet("/health/live", () => Results.Ok(new
{
    service = Environment.GetEnvironmentVariable("SERVICE_NAME") ?? "auth",
    status = "ok",
}));

app.MapGet("/health/ready", async (AuthDbContext dbContext, CancellationToken cancellationToken) =>
{
    var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);

    return canConnect
        ? Results.Ok(new
        {
            service = Environment.GetEnvironmentVariable("SERVICE_NAME") ?? "auth",
            status = "ready",
        })
        : Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
});

app.MapAuthEndpoints();

await app.Services.InitializeAuthDatabaseAsync();

app.Run();
