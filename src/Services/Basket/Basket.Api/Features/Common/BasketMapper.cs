namespace Basket.Api.Features.Common;

public static class BasketMapper
{
    public static BasketResponse ToResponse(this ShoppingBasket basket)
    {
        var items = basket.Items
            .Select(x => new BasketItemResponse(x.ProductId, x.Quantity, x.CreatedAt, x.UpdatedAt))
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
