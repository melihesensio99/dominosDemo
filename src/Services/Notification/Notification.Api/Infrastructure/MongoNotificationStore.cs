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
        return await AddAsync(
            Guid.NewGuid(),
            recipientId,
            message,
            status,
            cancellationToken);
    }

    public async Task<NotificationDocument> AddAsync(
        Guid eventId,
        string recipientId,
        string message,
        string status = "queued",
        CancellationToken cancellationToken = default)
    {
        var eventKey = eventId.ToString("N");
        var notification = new NotificationDocument
        {
            Id = eventKey,
            EventId = eventKey,
            RecipientId = recipientId,
            Message = message,
            Status = status,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        await collection.ReplaceOneAsync(
            item => item.EventId == eventKey,
            notification,
            new ReplaceOptions { IsUpsert = true },
            cancellationToken);

        return notification;
    }
}
