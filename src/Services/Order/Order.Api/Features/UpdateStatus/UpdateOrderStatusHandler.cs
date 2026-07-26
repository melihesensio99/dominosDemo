using Order.Api.Abstractions;
using Order.Api.Domain;
using Order.Api.Features.Common;
using Order.Api.Infrastructure;

namespace Order.Api.Features.UpdateStatus;

public sealed class UpdateOrderStatusHandler(
    IOrderRepository orderRepository,
    IHubContext<OrderTrackingHub> hubContext,
    ILogger<UpdateOrderStatusHandler> logger)
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

        try
        {
            await hubContext.Clients.User(order.CustomerId).SendAsync(
                "OrderStatusChanged",
                new
                {
                    orderId = order.Id,
                    status = response.Status,
                    updatedAt = response.UpdatedAt,
                },
                cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "Order status was saved but live notification failed for {OrderId}.", order.Id);
        }

        return Result<OrderResponse>.Success(response);
    }
}
