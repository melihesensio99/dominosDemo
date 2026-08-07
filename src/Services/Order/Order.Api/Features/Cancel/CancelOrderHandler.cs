using BuildingBlocks.Common;
using Order.Api.Abstractions;
using Order.Api.Features.Shared;

namespace Order.Api.Features.Cancel;

public sealed class CancelOrderHandler(
    IOrderRepository orderRepository) : IRequestHandler<CancelOrderCommand, Result<OrderResponse>>
{
    public async Task<Result<OrderResponse>> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.Id, cancellationToken);
        if (order is null)
        {
            return Result<OrderResponse>.NotFound("order.not_found", "Order was not found.");
        }

        if (!order.CustomerId.Equals(request.CustomerId, StringComparison.OrdinalIgnoreCase))
        {
            return Result<OrderResponse>.Forbidden("order.forbidden", "You cannot cancel another customer's order.");
        }

        if (!order.CanBeCancelled)
        {
            return Result<OrderResponse>.Conflict(
                "order.cancellation_closed",
                "The order cannot be cancelled after preparation has started.");
        }

        if (!order.Cancel())
        {
            return Result<OrderResponse>.Conflict("order.already_cancelled", "Order is already cancelled.");
        }

        await orderRepository.SaveAsync(order, cancellationToken);

        return Result<OrderResponse>.Success(OrderMapper.ToResponse(order));
    }
}
