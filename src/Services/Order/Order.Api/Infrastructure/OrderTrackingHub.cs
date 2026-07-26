using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Order.Api.Infrastructure;

[Authorize]
public sealed class OrderTrackingHub : Hub
{
}
