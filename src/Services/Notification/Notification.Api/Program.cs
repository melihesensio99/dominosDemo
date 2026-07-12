using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var notifications = new ConcurrentDictionary<string, NotificationDto>();

app.MapGet("/", () => Results.Ok(new
{
    service = Environment.GetEnvironmentVariable("SERVICE_NAME") ?? "notification",
    status = "ok",
}));

app.MapGet("/health", () => Results.Ok(new
{
    service = Environment.GetEnvironmentVariable("SERVICE_NAME") ?? "notification",
    status = "ok",
}));

app.MapGet("/notifications", () => Results.Ok(new { items = notifications.Values.OrderByDescending(item => item.CreatedAt) }));

app.MapPost("/notifications", (CreateNotificationRequest request) =>
{
    var id = Guid.NewGuid().ToString("N");
    var notification = new NotificationDto(id, request.RecipientId, request.Message, "queued", DateTimeOffset.UtcNow);
    notifications[id] = notification;
    return Results.Accepted($"/notifications/{id}", notification);
});

app.MapGet("/notifications/{id}", (string id) =>
{
    return notifications.TryGetValue(id, out var notification)
        ? Results.Ok(notification)
        : Results.NotFound(new { error = "notification-not-found", id });
});

app.Run();

internal sealed record CreateNotificationRequest(string RecipientId, string Message);

internal sealed record NotificationDto(
    string Id,
    string RecipientId,
    string Message,
    string Status,
    DateTimeOffset CreatedAt);
