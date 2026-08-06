using BuildingBlocks.Common;

namespace Basket.Api.Features.Items.Add;

public sealed class AddBasketItemHandler(
    IBasketRepository basketRepository,
    IInventoryStockClient stockClient,
    ICatalogProductClient catalogProductClient) : IRequestHandler<AddBasketItemCommand, Result<BasketResponse>>
{
    public async Task<Result<BasketResponse>> Handle(AddBasketItemCommand request, CancellationToken cancellationToken)
    {
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

        var stockKeyResult = ResolveStockKey(product, selectedOptions);
        if (!stockKeyResult.IsSuccess || stockKeyResult.Value is null)
        {
            return Result<BasketResponse>.Validation(
                stockKeyResult.Error?.Code ?? "basket.stock_mapping_missing",
                stockKeyResult.Error?.Message ?? "Stock mapping is missing.");
        }

        var stockKey = stockKeyResult.Value;
        var stockResult = await stockClient.GetStockAsync(stockKey, cancellationToken);
        if (!stockResult.IsSuccess || stockResult.Value is null)
        {
            return Result<BasketResponse>.Failure(stockResult.Error?.Code ?? "stock_error", stockResult.Error?.Message ?? "Stock could not be loaded.");
        }

        var basket = await basketRepository.GetAsync(request.CustomerId, cancellationToken) ?? ShoppingBasket.Create(request.CustomerId);
        basket.AddItem(product, selectedOptions, stockKey, request.Quantity);

        var requestedFromPool = basket.Items
            .Where(item => item.StockKey == stockKey)
            .Sum(item => item.Quantity);
        if (requestedFromPool > stockResult.Value.Available)
        {
            return Result<BasketResponse>.Validation(
                "basket.stock_not_enough",
                $"Only {stockResult.Value.Available} items are available for {stockKey}.");
        }

        await basketRepository.SaveAsync(basket, cancellationToken);
        return Result<BasketResponse>.Success(basket.ToResponse());
    }

    private static Result<string> ResolveStockKey(
        CatalogProductSnapshot product,
        IReadOnlyCollection<CatalogOptionSnapshot> selectedOptions)
    {
        if (product.InventoryTrackingType.Equals("direct", StringComparison.OrdinalIgnoreCase))
        {
            return Result<string>.Success(product.InventoryKey ?? product.Id);
        }

        var stockKeys = selectedOptions
            .Select(option => option.InventoryKey)
            .Where(key => !string.IsNullOrWhiteSpace(key))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return stockKeys.Length == 1
            ? Result<string>.Success(stockKeys[0]!)
            : Result<string>.Validation(
                "basket.stock_mapping_missing",
                "The selected product size is not connected to a dough stock.");
    }
}
