using MongoDB.Driver;

namespace Notification.Api.Infrastructure;

public static class NotificationMongoBootstrapper
{
    public static async Task EnsureIndexesAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var collection = scope.ServiceProvider.GetRequiredService<IMongoCollection<NotificationDocument>>();

        var index = new CreateIndexModel<NotificationDocument>(
            Builders<NotificationDocument>.IndexKeys.Ascending(item => item.EventId),
            new CreateIndexOptions
            {
                Name = "ux_notifications_event_id",
                Unique = true,
            });

        await collection.Indexes.CreateOneAsync(index, cancellationToken: cancellationToken);
    }
}
