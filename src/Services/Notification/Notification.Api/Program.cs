using Notification.Api.Features;
using Notification.Api.Infrastructure.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddNotificationModule(builder.Configuration);

var app = builder.Build();

app.UseCors("RealtimeClients");
app.UseAuthentication();
app.UseAuthorization();

app.MapNotificationEndpoints();

app.Run();
