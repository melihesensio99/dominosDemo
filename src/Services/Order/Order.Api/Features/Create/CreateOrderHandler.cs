using BuildingBlocks.Common;
using System.Text.Json;
using Order.Api.Abstractions;
using Order.Api.Domain;
using Order.Api.Features.Common;
using OrderItem = Order.Api.Domain.OrderItem;
using OrderEntity = Order.Api.Domain.Order;

namespace Order.Api.Features.Create;

public sealed class CreateOrderHandler(
    IOrderRepository orderRepository) : IRequestHandler<CreateOrderCommand, Result<OrderResponse>>
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
            request.PaymentMethod);

        await orderRepository.SaveAsync(order, cancellationToken);

        return Result<OrderResponse>.Success(OrderMapper.ToResponse(order));
    }
}
