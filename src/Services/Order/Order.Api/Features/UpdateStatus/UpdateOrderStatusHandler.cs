using Order.Api.Abstractions;
using Order.Api.Domain;
using Order.Api.Features.Shared;

namespace Order.Api.Features.UpdateStatus;

public sealed class UpdateOrderStatusHandler(
    IOrderRepository orderRepository)
    : IRequestHandler<UpdateOrderStatusCommand, Result<OrderResponse>>
{
    public async Task<Result<OrderResponse>> Handle(
        UpdateOrderStatusCommand request,
        CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
        {
            return Result<OrderResponse>.NotFound("order.not_found", "Order was not found.");
        }

        if (!Enum.TryParse<OrderStatus>(request.Status, true, out var newStatus)
            || newStatus == OrderStatus.Pending)
        {
            return Result<OrderResponse>.Validation(
                "order.invalid_status",
                "Status must be one of: confirmed, preparing, shipped, delivered, cancelled.");
        }

        if (!order.ChangeStatus(newStatus))
        {
            return Result<OrderResponse>.Conflict(
                "order.invalid_status_transition",
                $"Order cannot move from {order.Status.ToString().ToLowerInvariant()} to {newStatus.ToString().ToLowerInvariant()}.");
        }

        await orderRepository.SaveAsync(order, cancellationToken);
        var response = OrderMapper.ToResponse(order);

        return Result<OrderResponse>.Success(response);
    }
}
