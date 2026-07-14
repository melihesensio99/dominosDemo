using MongoDB.Driver;

namespace Notification.Api.Infrastructure;

public sealed class MongoNotificationStore(IMongoCollection<NotificationDocument> collection)
{
    public async Task<IReadOnlyCollection<NotificationDocument>> GetAllAsync(CancellationToken cancellationToken)
    {
        var notifications = await collection
            .Find(Builders<NotificationDocument>.Filter.Empty)
            .SortByDescending(item => item.CreatedAt)
            .ToListAsync(cancellationToken);

        return notifications;
    }

    public async Task<NotificationDocument?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await collection.Find(item => item.Id == id).FirstOrDefaultAsync(cancellationToken);

    public async Task<NotificationDocument> AddAsync(string recipientId, string message, string status = "queued", CancellationToken cancellationToken = default)
    {
        var notification = new NotificationDocument
        {
            Id = Guid.NewGuid().ToString("N"),
            RecipientId = recipientId,
            Message = message,
            Status = status,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        await collection.InsertOneAsync(notification, cancellationToken: cancellationToken);
        return notification;
    }
}
