using Notification.Api.Infrastructure;
using Notification.Api.Infrastructure.Realtime;

namespace Notification.Api.Features;

public static class NotificationEndpoints
{
    public static IEndpointRouteBuilder MapNotificationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/notifications", async (MongoNotificationStore store, CancellationToken cancellationToken) =>
        {
            var items = await store.GetAllAsync(cancellationToken);
            return Results.Ok(new { items });
        });

        app.MapPost("/notifications", async (
            CreateNotificationRequest request,
            MongoNotificationStore store,
            CancellationToken cancellationToken) =>
        {
            var notification = await store.AddAsync(
                request.RecipientId,
                request.Message,
                cancellationToken: cancellationToken);

            return Results.Accepted($"/notifications/{notification.Id}", notification);
        });

        app.MapGet("/notifications/{id}", async (
            string id,
            MongoNotificationStore store,
            CancellationToken cancellationToken) =>
        {
            return await store.GetByIdAsync(id, cancellationToken) is { } notification
                ? Results.Ok(notification)
                : Results.NotFound(new { error = "notification-not-found", id });
        });

        app.MapHub<NotificationHub>("/hubs/notifications");
        return app;
    }
}

public sealed record CreateNotificationRequest(string RecipientId, string Message);
