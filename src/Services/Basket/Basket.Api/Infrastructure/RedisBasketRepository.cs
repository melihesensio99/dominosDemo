using System.Text.Json;
using StackExchange.Redis;

namespace Basket.Api.Infrastructure;

public sealed class RedisBasketRepository(IConnectionMultiplexer connectionMultiplexer) : IBasketRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
    };

    private IDatabase Database => connectionMultiplexer.GetDatabase();

    public async Task<ShoppingBasket?> GetAsync(string customerId, CancellationToken cancellationToken)
    {
        _ = cancellationToken;

        var data = await Database.StringGetAsync(GetKey(customerId));
        if (data.IsNullOrEmpty)
        {
            return null;
        }

        return JsonSerializer.Deserialize<ShoppingBasket>(data.ToString(), JsonOptions);
    }

    public async Task SaveAsync(ShoppingBasket basket, CancellationToken cancellationToken)
    {
        _ = cancellationToken;

        basket.UpdatedAt = DateTimeOffset.UtcNow;
        var payload = JsonSerializer.Serialize(basket, JsonOptions);
        await Database.StringSetAsync(GetKey(basket.CustomerId), payload);
    }

    public async Task DeleteAsync(string customerId, CancellationToken cancellationToken)
    {
        _ = cancellationToken;

        await Database.KeyDeleteAsync(GetKey(customerId));
    }

    private static string GetKey(string customerId) => $"basket:{customerId}";
}
