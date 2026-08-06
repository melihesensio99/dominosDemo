namespace Notification.Api.Abstractions.Realtime;

public interface IRealtimeNotificationPublisher
{
    Task NotifyNewOrderAsync(NewOrderNotification notification);

    Task NotifyOrderStatusChangedAsync(OrderStatusChangedNotification notification);

    Task NotifyLowStockAsync(LowStockNotification notification);
}
