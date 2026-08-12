using Auth.Api.Features;
using Auth.Api.Infrastructure.Configuration;
using BuildingBlocks.Web;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();
    builder.Services.AddDataProtection().UseEphemeralDataProtectionProvider();
}

builder.Services.AddAuthModule(builder.Configuration);

var app = builder.Build();

app.UseGlobalExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();

await app.Services.InitializeAuthDatabaseAsync();

app.Run();
