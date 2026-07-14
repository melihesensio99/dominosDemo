using BuildingBlocks.Common;
using MediatR;
using Order.Api.Domain;
using Order.Api.Features.Common;

namespace Order.Api.Features.Create;

public sealed record CreateOrderCommand(
    string CustomerId,
    List<CreateOrderItemRequest> Items,
    AddressRequest ShippingAddress,
    AddressRequest BillingAddress,
    PaymentMethod PaymentMethod) : IRequest<Result<OrderResponse>>;

public sealed record CreateOrderItemRequest(string ProductId, int Quantity);

public sealed record AddressRequest(
    string Street,
    string District,
    string City,
    string PostalCode,
    string Country);
