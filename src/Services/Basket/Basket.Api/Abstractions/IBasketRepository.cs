namespace Basket.Api.Abstractions;

public interface IBasketRepository
{
    Task<ShoppingBasket?> GetAsync(string customerId, CancellationToken cancellationToken);

    Task SaveAsync(ShoppingBasket basket, CancellationToken cancellationToken);

    Task DeleteAsync(string customerId, CancellationToken cancellationToken);
}
