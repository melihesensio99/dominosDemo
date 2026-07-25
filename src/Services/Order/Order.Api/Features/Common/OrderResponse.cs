namespace Order.Api.Features.Common;

public sealed record OrderItemResponse(string ProductId, int Quantity, IReadOnlyList<Guid> SelectedOptionIds);

public sealed record AddressResponse(
    string Street,
    string District,
    string City,
    string PostalCode,
    string Country);

public sealed record PaymentResponse(string Method, string Status);

public sealed record OrderResponse(
    string Id,
    string CustomerId,
    List<OrderItemResponse> Items,
    AddressResponse ShippingAddress,
    AddressResponse BillingAddress,
    PaymentResponse Payment,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
