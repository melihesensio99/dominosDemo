using Basket.Api.Features;
using Basket.Api.Infrastructure.Configuration;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();
    builder.Services.AddDataProtection().UseEphemeralDataProtectionProvider();
}

builder.Services.AddBasketModule(builder.Configuration);

var app = builder.Build();

app.UseGlobalExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

app.MapBasketEndpoints();

app.Run();
