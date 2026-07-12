using BuildingBlocks.Common;

namespace Basket.Api.Features.Get;

public sealed class GetBasketHandler(IBasketRepository basketRepository) : IRequestHandler<GetBasketQuery, Result<BasketResponse>>
{
    public async Task<Result<BasketResponse>> Handle(GetBasketQuery request, CancellationToken cancellationToken)
    {
        var basket = await basketRepository.GetAsync(request.CustomerId, cancellationToken);
        basket ??= ShoppingBasket.Create(request.CustomerId);

        return Result<BasketResponse>.Success(basket.ToResponse());
    }
}
