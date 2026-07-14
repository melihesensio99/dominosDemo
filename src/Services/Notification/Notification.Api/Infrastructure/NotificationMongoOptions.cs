namespace Notification.Api.Infrastructure;

public sealed class NotificationMongoOptions
{
    public string ConnectionString { get; init; } = "mongodb://localhost:27017";

    public string Database { get; init; } = "notification_db";

    public string Collection { get; init; } = "notifications";
}
