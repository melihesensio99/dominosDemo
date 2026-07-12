namespace Basket.Api.Features.Common;

public sealed record BasketItemResponse(string ProductId, int Quantity, DateTimeOffset CreatedAt, DateTimeOffset? UpdatedAt);

public sealed record BasketResponse(
    string CustomerId,
    IReadOnlyList<BasketItemResponse> Items,
    int ItemCount,
    int TotalQuantity,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
