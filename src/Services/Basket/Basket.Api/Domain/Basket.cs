namespace Basket.Api.Domain;

public sealed class ShoppingBasket
{
    public string CustomerId { get; set; } = string.Empty;

    public List<BasketItem> Items { get; set; } = [];

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public static ShoppingBasket Create(string customerId) => new()
    {
        CustomerId = customerId,
        CreatedAt = DateTimeOffset.UtcNow,
        UpdatedAt = DateTimeOffset.UtcNow,
    };

    public void AddItem(string productId, int quantity)
    {
        var item = Items.FirstOrDefault(x => x.ProductId == productId);
        if (item is null)
        {
            Items.Add(new BasketItem
            {
                ProductId = productId,
                Quantity = quantity,
            });
        }
        else
        {
            item.Quantity += quantity;
            item.UpdatedAt = DateTimeOffset.UtcNow;
        }

        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public bool UpdateItemQuantity(string productId, int quantity)
    {
        var item = Items.FirstOrDefault(x => x.ProductId == productId);
        if (item is null)
        {
            return false;
        }

        item.Quantity = quantity;
        item.UpdatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
        return true;
    }

    public bool RemoveItem(string productId)
    {
        var item = Items.FirstOrDefault(x => x.ProductId == productId);
        if (item is null)
        {
            return false;
        }

        Items.Remove(item);
        UpdatedAt = DateTimeOffset.UtcNow;
        return true;
    }

    public int GetTotalQuantity() => Items.Sum(x => x.Quantity);
}
