using Order.Api.Features;
using Order.Api.Infrastructure.Configuration;
using Order.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();
    builder.Services.AddDataProtection().UseEphemeralDataProtectionProvider();
}

builder.Services.AddOrderModule(builder.Configuration);

var app = builder.Build();

app.UseGlobalExceptionHandler();
app.UseCors("AdminPanel");
app.UseAuthentication();
app.UseAuthorization();

app.MapOrderEndpoints();

await app.Services.InitializeOrderDatabaseAsync();

app.Run();
