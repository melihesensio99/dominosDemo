using Notification.Api.Features;
using Notification.Api.Infrastructure.Configuration;
using Notification.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddNotificationModule(builder.Configuration);

var app = builder.Build();

await app.Services.EnsureIndexesAsync();

app.UseCors("RealtimeClients");
app.UseAuthentication();
app.UseAuthorization();

app.MapNotificationEndpoints();

app.Run();
