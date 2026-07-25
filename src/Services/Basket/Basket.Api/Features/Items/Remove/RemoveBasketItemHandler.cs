using BuildingBlocks.Common;

namespace Basket.Api.Features.Items.Remove;

public sealed class RemoveBasketItemHandler(IBasketRepository basketRepository) : IRequestHandler<RemoveBasketItemCommand, Result<BasketResponse>>
{
    public async Task<Result<BasketResponse>> Handle(RemoveBasketItemCommand request, CancellationToken cancellationToken)
    {
        var basket = await basketRepository.GetAsync(request.CustomerId, cancellationToken);
        if (basket is null)
        {
            return Result<BasketResponse>.Success(ShoppingBasket.Create(request.CustomerId).ToResponse());
        }

        if (!basket.RemoveItem(request.ItemId))
        {
            return Result<BasketResponse>.NotFound("basket.item_not_found", "Basket item was not found.");
        }

        if (basket.Items.Count == 0)
        {
            await basketRepository.DeleteAsync(request.CustomerId, cancellationToken);
            return Result<BasketResponse>.Success(ShoppingBasket.Create(request.CustomerId).ToResponse());
        }

        await basketRepository.SaveAsync(basket, cancellationToken);
        return Result<BasketResponse>.Success(basket.ToResponse());
    }
}
