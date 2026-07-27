using BuildingBlocks.Common;
using System.Text.Json;
using Order.Api.Abstractions;
using Order.Api.Domain;
using Order.Api.Features.Common;
using Order.Api.Infrastructure;
using OrderItem = Order.Api.Domain.OrderItem;
using OrderEntity = Order.Api.Domain.Order;
using Microsoft.AspNetCore.SignalR;

namespace Order.Api.Features.Create;

public sealed class CreateOrderHandler(
    IOrderRepository orderRepository,
    IHubContext<OrderTrackingHub> hubContext,
    ILogger<CreateOrderHandler> logger) : IRequestHandler<CreateOrderCommand, Result<OrderResponse>>
{
    public async Task<Result<OrderResponse>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = OrderEntity.Create(
            request.CustomerId,
            request.Items.Select(item => new OrderItem(
                item.ProductId,
                item.Quantity,
                JsonSerializer.Serialize(item.SelectedOptionIds ?? []))),
            Address.Create(
                request.ShippingAddress.Street,
                request.ShippingAddress.District,
                request.ShippingAddress.City,
                request.ShippingAddress.PostalCode,
                request.ShippingAddress.Country),
            Address.Create(
                request.BillingAddress.Street,
                request.BillingAddress.District,
                request.BillingAddress.City,
                request.BillingAddress.PostalCode,
            request.BillingAddress.Country),
            request.PaymentMethod,
            request.Note);

        await orderRepository.SaveAsync(order, cancellationToken);

        try
        {
            await hubContext.Clients.Group(OrderTrackingHub.AdminGroup).SendAsync(
                "NewOrderCreated",
                new
                {
                    orderId = order.Id,
                    customerId = order.CustomerId,
                    status = order.Status.ToString().ToLowerInvariant(),
                    itemCount = order.Items.Sum(item => item.Quantity),
                    createdAt = order.CreatedAt,
                },
                cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "Order was saved but admin live notification failed for {OrderId}.", order.Id);
        }

        return Result<OrderResponse>.Success(OrderMapper.ToResponse(order));
    }
}
