namespace Basket.Api.Features.Common;

public static class BasketMapper
{
    public static BasketResponse ToResponse(this ShoppingBasket basket)
    {
        var items = basket.Items
            .Select(x => new BasketItemResponse(
                x.Id,
                x.ProductId,
                x.ProductName,
                x.StockKey,
                x.BasePrice,
                x.UnitPrice,
                x.TotalPrice,
                x.Quantity,
                x.SelectedOptions
                    .Select(option => new SelectedBasketOptionResponse(
                        option.OptionId,
                        option.GroupName,
                        option.Name,
                        option.PriceAdjustment))
                    .ToArray(),
                x.CreatedAt,
                x.UpdatedAt))
            .ToArray();

        return new BasketResponse(
            basket.CustomerId,
            items,
            items.Length,
            basket.GetTotalQuantity(),
            basket.CreatedAt,
            basket.UpdatedAt);
    }
}
