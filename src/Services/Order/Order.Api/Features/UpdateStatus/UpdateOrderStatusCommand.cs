using Order.Api.Features.Common;

namespace Order.Api.Features.UpdateStatus;

public sealed record UpdateOrderStatusCommand(
    string OrderId,
    string Status) : IRequest<Result<OrderResponse>>;
