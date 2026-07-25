namespace Basket.Api.Domain;

public sealed class BasketItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string ProductId { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public decimal BasePrice { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; set; }

    public string ConfigurationKey { get; set; } = string.Empty;

    public List<SelectedBasketOption> SelectedOptions { get; set; } = [];

    public int Quantity { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? UpdatedAt { get; set; }
}

public sealed class SelectedBasketOption
{
    public Guid OptionId { get; set; }

    public string GroupName { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public decimal PriceAdjustment { get; set; }
}
