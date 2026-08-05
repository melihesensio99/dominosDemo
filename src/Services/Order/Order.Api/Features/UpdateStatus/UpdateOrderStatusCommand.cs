using Order.Api.Features.Shared;

namespace Order.Api.Features.UpdateStatus;

public sealed record UpdateOrderStatusCommand(
    string OrderId,
    string Status) : IRequest<Result<OrderResponse>>;
