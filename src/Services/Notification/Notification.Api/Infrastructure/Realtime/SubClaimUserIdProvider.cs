using Microsoft.AspNetCore.SignalR;

namespace Notification.Api.Infrastructure.Realtime;

public sealed class SubClaimUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection) =>
        connection.User?.FindFirst("sub")?.Value;
}
