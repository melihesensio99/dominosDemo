using BuildingBlocks.Common;

namespace Basket.Api.Features.Items.Add;

public sealed class AddBasketItemHandler(
    IBasketRepository basketRepository,
    IInventoryStockClient stockClient,
    ICatalogProductClient catalogProductClient) : IRequestHandler<AddBasketItemCommand, Result<BasketResponse>>
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

        var productResult = await catalogProductClient.GetProductAsync(request.ProductId, cancellationToken);
        if (!productResult.IsSuccess || productResult.Value is null)
        {
            return Result<BasketResponse>.Failure(
                productResult.Error?.Code ?? "catalog_error",
                productResult.Error?.Message ?? "Product could not be loaded.");
        }

        var product = productResult.Value;
        if (!product.IsActive)
        {
            return Result<BasketResponse>.Validation("basket.product_inactive", "The selected product is not available.");
        }

        var selectedOptionIds = request.SelectedOptionIds?.ToHashSet() ?? [];
        var selectedOptions = product.OptionGroups
            .SelectMany(group => group.Options)
            .Where(option => selectedOptionIds.Contains(option.Id) && option.IsActive)
            .ToArray();

        if (selectedOptions.Length != selectedOptionIds.Count)
        {
            return Result<BasketResponse>.Validation("basket.invalid_options", "One or more selected options are invalid.");
        }

        foreach (var group in product.OptionGroups)
        {
            var selectedCount = selectedOptions.Count(option => option.GroupName == group.Name);
            if (group.IsRequired && selectedCount == 0)
            {
                return Result<BasketResponse>.Validation("basket.required_option_missing", $"An option must be selected for {group.Name}.");
            }

            if (group.SelectionType == "single" && selectedCount > 1)
            {
                return Result<BasketResponse>.Validation("basket.multiple_options_selected", $"Only one option can be selected for {group.Name}.");
            }
        }

        var basket = await basketRepository.GetAsync(request.CustomerId, cancellationToken) ?? ShoppingBasket.Create(request.CustomerId);
        basket.AddItem(product, selectedOptions, request.Quantity);

        var basketItem = basket.Items.First(x => x.ProductId == request.ProductId);
        if (basketItem.Quantity > stock.Available)
        {
            return Result<BasketResponse>.Validation("basket.stock_not_enough", $"Only {stock.Available} items are available for product {request.ProductId}.");
        }

        await basketRepository.SaveAsync(basket, cancellationToken);
        return Result<BasketResponse>.Success(basket.ToResponse());
    }
}
