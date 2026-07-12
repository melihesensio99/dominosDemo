using BuildingBlocks.Common;

namespace Basket.Api.Features.Items.Update;

public sealed class UpdateBasketItemQuantityHandler(IBasketRepository basketRepository, IInventoryStockClient stockClient) : IRequestHandler<UpdateBasketItemQuantityCommand, Result<BasketResponse>>
{
    public async Task<Result<BasketResponse>> Handle(UpdateBasketItemQuantityCommand request, CancellationToken cancellationToken)
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

        var basket = await basketRepository.GetAsync(request.CustomerId, cancellationToken);
        if (basket is null)
        {
            return Result<BasketResponse>.NotFound("basket.not_found", "Basket was not found.");
        }

        if (!basket.UpdateItemQuantity(request.ProductId, request.Quantity))
        {
            return Result<BasketResponse>.NotFound("basket.item_not_found", "Basket item was not found.");
        }

        await basketRepository.SaveAsync(basket, cancellationToken);
        return Result<BasketResponse>.Success(basket.ToResponse());
    }
}
