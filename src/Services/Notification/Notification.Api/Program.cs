using Notification.Api.Features;

var builder = WebApplication.CreateBuilder(args);

NotificationModule.ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

app.UseCors("RealtimeClients");
app.UseAuthentication();
app.UseAuthorization();

app.MapNotificationEndpoints();

app.Run();
