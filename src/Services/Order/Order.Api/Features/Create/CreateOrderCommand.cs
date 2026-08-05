using BuildingBlocks.Common;
using MediatR;
using Order.Api.Domain;
using Order.Api.Features.Shared;

namespace Order.Api.Features.Create;

public sealed record CreateOrderRequest(
    List<CreateOrderItemRequest> Items,
    AddressRequest ShippingAddress,
    AddressRequest BillingAddress,
    PaymentMethod PaymentMethod,
    string? Note = null);

public sealed record CreateOrderCommand(
    string CustomerId,
    List<CreateOrderItemRequest> Items,
    AddressRequest ShippingAddress,
    AddressRequest BillingAddress,
    PaymentMethod PaymentMethod,
    string? Note) : IRequest<Result<OrderResponse>>;

public sealed record CreateOrderItemRequest(
    string ProductId,
    int Quantity,
    List<Guid>? SelectedOptionIds = null);

public sealed record AddressRequest(
    string Street,
    string District,
    string City,
    string PostalCode,
    string Country);
