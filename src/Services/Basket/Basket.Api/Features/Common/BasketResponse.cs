namespace Basket.Api.Features.Common;

public sealed record BasketItemResponse(
    Guid Id,
    string ProductId,
    string ProductName,
    string StockKey,
    decimal BasePrice,
    decimal UnitPrice,
    decimal TotalPrice,
    int Quantity,
    IReadOnlyList<SelectedBasketOptionResponse> SelectedOptions,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);

public sealed record SelectedBasketOptionResponse(
    Guid OptionId,
    string GroupName,
    string Name,
    decimal PriceAdjustment);

public sealed record BasketResponse(
    string CustomerId,
    IReadOnlyList<BasketItemResponse> Items,
    int ItemCount,
    int TotalQuantity,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
