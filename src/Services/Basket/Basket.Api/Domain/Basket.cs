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

    public void AddItem(
        CatalogProductSnapshot product,
        IReadOnlyList<CatalogOptionSnapshot> selectedOptions,
        string stockKey,
        int quantity)
    {
        var configurationKey = BuildConfigurationKey(product.Id, selectedOptions);
        var unitPrice = product.Price + selectedOptions.Sum(option => option.PriceAdjustment);
        var item = Items.FirstOrDefault(x => x.ConfigurationKey == configurationKey);
        if (item is null)
        {
            item = new BasketItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                StockKey = stockKey,
                BasePrice = product.Price,
                UnitPrice = unitPrice,
                TotalPrice = unitPrice * quantity,
                ConfigurationKey = configurationKey,
                SelectedOptions = selectedOptions.Select(option => new SelectedBasketOption
                {
                    OptionId = option.Id,
                    GroupName = option.GroupName,
                    Name = option.Name,
                    PriceAdjustment = option.PriceAdjustment,
                }).ToList(),
                Quantity = quantity,
            };

            Items.Add(item);
        }
        else
        {
            item.Quantity += quantity;
            item.TotalPrice = item.UnitPrice * item.Quantity;
            item.UpdatedAt = DateTimeOffset.UtcNow;
        }

        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public bool UpdateItemQuantity(Guid itemId, int quantity)
    {
        var item = Items.FirstOrDefault(x => x.Id == itemId);
        if (item is null)
        {
            return false;
        }

        item.Quantity = quantity;
        item.TotalPrice = item.UnitPrice * quantity;
        item.UpdatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
        return true;
    }

    public bool RemoveItem(Guid itemId)
    {
        var item = Items.FirstOrDefault(x => x.Id == itemId);
        if (item is null)
        {
            return false;
        }

        Items.Remove(item);
        UpdatedAt = DateTimeOffset.UtcNow;
        return true;
    }

    public int GetTotalQuantity() => Items.Sum(x => x.Quantity);

    private static string BuildConfigurationKey(string productId, IReadOnlyList<CatalogOptionSnapshot> selectedOptions) =>
        $"{productId}:{string.Join(',', selectedOptions.Select(option => option.Id).OrderBy(id => id))}";
}
