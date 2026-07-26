using Auth.Api.Features;
using BuildingBlocks.Web;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();
    builder.Services.AddDataProtection().UseEphemeralDataProtectionProvider();
}

AuthModule.ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

app.UseGlobalExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();

await app.Services.InitializeAuthDatabaseAsync();

app.Run();
