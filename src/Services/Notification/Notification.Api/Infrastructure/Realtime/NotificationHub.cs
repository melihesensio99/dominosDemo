using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Notification.Api.Abstractions.Realtime;

namespace Notification.Api.Infrastructure.Realtime;

[Authorize]
public sealed class NotificationHub : Hub<INotificationClient>
{
    public const string AdminGroup = "notification-admins";

    public override async Task OnConnectedAsync()
    {
        if (Context.User?.IsInRole("Admin") == true)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, AdminGroup);
        }

        await base.OnConnectedAsync();
    }
}
