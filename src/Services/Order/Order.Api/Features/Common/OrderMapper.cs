using OrderEntity = Order.Api.Domain.Order;

using System.Text.Json;

namespace Order.Api.Features.Common;

public static class OrderMapper
{
    public static OrderResponse ToResponse(OrderEntity order) =>
        new(
            order.Id,
            order.CustomerId,
            order.Items.Select(item => new OrderItemResponse(
                item.ProductId,
                item.Quantity,
                JsonSerializer.Deserialize<List<Guid>>(item.SelectedOptionIdsJson) ?? [])).ToList(),
            new AddressResponse(
                order.ShippingAddress.Street,
                order.ShippingAddress.District,
                order.ShippingAddress.City,
                order.ShippingAddress.PostalCode,
                order.ShippingAddress.Country),
            new AddressResponse(
                order.BillingAddress.Street,
                order.BillingAddress.District,
                order.BillingAddress.City,
                order.BillingAddress.PostalCode,
                order.BillingAddress.Country),
            new PaymentResponse(
                order.Payment.Method.ToString().ToLowerInvariant(),
                order.Payment.Status.ToString().ToLowerInvariant()),
            order.Status.ToString().ToLowerInvariant(),
            order.CreatedAt,
            order.UpdatedAt);
}
