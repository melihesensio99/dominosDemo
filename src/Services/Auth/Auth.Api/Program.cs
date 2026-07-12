using Auth.Api.Features;
using BuildingBlocks.Web;

var builder = WebApplication.CreateBuilder(args);

AuthModule.ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

app.UseValidationExceptionHandler();

app.MapGet("/", () => Results.Ok(new
{
    service = Environment.GetEnvironmentVariable("SERVICE_NAME") ?? "auth",
    status = "ok",
}));

app.MapGet("/health", () => Results.Ok(new
{
    service = Environment.GetEnvironmentVariable("SERVICE_NAME") ?? "auth",
    status = "ok",
}));

app.MapAuthEndpoints();

await app.Services.InitializeAuthDatabaseAsync();

app.Run();
