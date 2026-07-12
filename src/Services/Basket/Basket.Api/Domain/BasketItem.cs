namespace Basket.Api.Domain;

public sealed class BasketItem
{
    public string ProductId { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? UpdatedAt { get; set; }
}
