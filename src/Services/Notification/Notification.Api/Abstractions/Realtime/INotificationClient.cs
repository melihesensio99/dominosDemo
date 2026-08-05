namespace Notification.Api.Abstractions.Realtime;

public interface INotificationClient
{
    Task NewOrderCreated(NewOrderNotification notification);

    Task OrderStatusChanged(OrderStatusChangedNotification notification);
}
