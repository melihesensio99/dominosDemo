using BuildingBlocks.Common;

namespace Basket.Api.Features.Items.Add;

public sealed class AddBasketItemHandler(IBasketRepository basketRepository, IInventoryStockClient stockClient) : IRequestHandler<AddBasketItemCommand, Result<BasketResponse>>
{
    public async Task<Result<BasketResponse>> Handle(AddBasketItemCommand request, CancellationToken cancellationToken)
    {
        var stockResult = await stockClient.GetStockAsync(request.ProductId, cancellationToken);
        if (!stockResult.IsSuccess || stockResult.Value is null)
        {
            return Result<BasketResponse>.Failure(stockResult.Error?.Code ?? "stock_error", stockResult.Error?.Message ?? "Stock could not be loaded.");
        }

        var stock = stockResult.Value;
        if (!stock.CanFit(request.Quantity))
        {
            return Result<BasketResponse>.Validation("basket.stock_not_enough", $"Only {stock.Available} items are available for product {request.ProductId}.");
        }

        var basket = await basketRepository.GetAsync(request.CustomerId, cancellationToken) ?? ShoppingBasket.Create(request.CustomerId);
        basket.AddItem(request.ProductId, request.Quantity);

        var basketItem = basket.Items.First(x => x.ProductId == request.ProductId);
        if (basketItem.Quantity > stock.Available)
        {
            return Result<BasketResponse>.Validation("basket.stock_not_enough", $"Only {stock.Available} items are available for product {request.ProductId}.");
        }

        await basketRepository.SaveAsync(basket, cancellationToken);
        return Result<BasketResponse>.Success(basket.ToResponse());
    }
}
