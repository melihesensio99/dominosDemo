namespace Catalog.Api.Features.Products.Common;

public static class ProductMapper
{
    public static ProductResponse ToResponse(Product product, string? categoryName = null) =>
        new(
            product.Id,
            product.Name,
            product.Description,
            product.ImageUrl,
            product.Price,
            product.Stock,
            product.CategoryId,
            categoryName ?? product.Category?.Name,
            product.OptionGroups
                .OrderBy(group => group.DisplayOrder)
                .Select(group => new ProductOptionGroupResponse(
                    group.Id,
                    group.Name,
                    group.SelectionType,
                    group.IsRequired,
                    group.DisplayOrder,
                    group.Options
                        .Where(option => option.IsActive)
                        .OrderBy(option => option.DisplayOrder)
                        .Select(option => new ProductOptionResponse(
                            option.Id,
                            option.Name,
                            option.PriceAdjustment,
                            option.IsDefault,
                            option.IsActive,
                            option.DisplayOrder))
                        .ToArray()))
                .ToArray(),
            product.IsActive,
            product.CreatedAt,
            product.UpdatedAt);
}
