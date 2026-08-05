using Microsoft.AspNetCore.SignalR;
using Notification.Api.Abstractions.Realtime;

namespace Notification.Api.Infrastructure.Realtime;

public sealed class SignalRNotificationPublisher(
    IHubContext<NotificationHub, INotificationClient> hubContext)
    : IRealtimeNotificationPublisher
{
    public Task NotifyNewOrderAsync(NewOrderNotification notification) =>
        hubContext.Clients
            .Group(NotificationHub.AdminGroup)
            .NewOrderCreated(notification);

    public Task NotifyOrderStatusChangedAsync(OrderStatusChangedNotification notification) =>
        Task.WhenAll(
            hubContext.Clients.User(notification.CustomerId)
                .OrderStatusChanged(notification),
            hubContext.Clients.Group(NotificationHub.AdminGroup)
                .OrderStatusChanged(notification));
}
