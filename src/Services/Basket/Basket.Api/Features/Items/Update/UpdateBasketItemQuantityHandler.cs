using BuildingBlocks.Common;

namespace Basket.Api.Features.Items.Update;

public sealed class UpdateBasketItemQuantityHandler(IBasketRepository basketRepository, IInventoryStockClient stockClient) : IRequestHandler<UpdateBasketItemQuantityCommand, Result<BasketResponse>>
{
    public async Task<Result<BasketResponse>> Handle(UpdateBasketItemQuantityCommand request, CancellationToken cancellationToken)
    {
        var basket = await basketRepository.GetAsync(request.CustomerId, cancellationToken);
        if (basket is null)
        {
            return Result<BasketResponse>.NotFound("basket.not_found", "Basket was not found.");
        }

        var basketItem = basket.Items.FirstOrDefault(item => item.Id == request.ItemId);
        if (basketItem is null)
        {
            return Result<BasketResponse>.NotFound("basket.item_not_found", "Basket item was not found.");
        }

        var stockResult = await stockClient.GetStockAsync(basketItem.StockKey, cancellationToken);
        if (!stockResult.IsSuccess || stockResult.Value is null)
        {
            return Result<BasketResponse>.Failure(stockResult.Error?.Code ?? "stock_error", stockResult.Error?.Message ?? "Stock could not be loaded.");
        }

        var stock = stockResult.Value;
        var otherItemsFromPool = basket.Items
            .Where(item => item.Id != basketItem.Id && item.StockKey == basketItem.StockKey)
            .Sum(item => item.Quantity);
        if (!stock.CanFit(otherItemsFromPool + request.Quantity))
        {
            return Result<BasketResponse>.Validation("basket.stock_not_enough", $"Only {stock.Available} items are available for {basketItem.StockKey}.");
        }

        if (!basket.UpdateItemQuantity(request.ItemId, request.Quantity))
        {
            return Result<BasketResponse>.NotFound("basket.item_not_found", "Basket item was not found.");
        }

        await basketRepository.SaveAsync(basket, cancellationToken);
        return Result<BasketResponse>.Success(basket.ToResponse());
    }
}
