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

    public async Task<(NotificationDocument Notification, bool IsNew)> AddAsync(
        string recipientId,
        string message,
        string status = "queued",
        CancellationToken cancellationToken = default)
    {
        return await AddAsync(Guid.NewGuid(), recipientId, message, status, cancellationToken);
    }

    public async Task<(NotificationDocument Notification, bool IsNew)> AddAsync(
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

        return await InsertOnceAsync(notification, cancellationToken);
    }

    public async Task<(NotificationDocument Notification, bool IsNew)> AddLowStockAsync(
        Guid eventId,
        string stockKey,
        string displayName,
        int available,
        int reorderLevel,
        CancellationToken cancellationToken)
    {
        var eventKey = eventId.ToString("N");
        var notification = new NotificationDocument
        {
            Id = eventKey,
            EventId = eventKey,
            RecipientId = "admins",
            Type = "low-stock",
            Title = "Kritik stok seviyesi",
            Message = $"{displayName} kritik seviyeye ulaştı. Kalan: {available}.",
            Status = "unread",
            StockKey = stockKey,
            Available = available,
            ReorderLevel = reorderLevel,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        return await InsertOnceAsync(notification, cancellationToken);
    }

    private async Task<(NotificationDocument Notification, bool IsNew)> InsertOnceAsync(
        NotificationDocument notification,
        CancellationToken cancellationToken)
    {
        try
        {
            await collection.InsertOneAsync(notification, cancellationToken: cancellationToken);
            return (notification, true);
        }
        catch (MongoWriteException exception) when (exception.WriteError?.Category == ServerErrorCategory.DuplicateKey)
        {
            var existing = await collection.Find(item => item.EventId == notification.EventId)
                .FirstOrDefaultAsync(cancellationToken);

            return (existing ?? notification, false);
        }
    }
}
