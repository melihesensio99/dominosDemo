namespace Catalog.Api.Features.Products.Common;

public static class ProductMapper
{
    public static ProductResponse ToResponse(Product product, string? categoryName = null) =>
        new(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.Stock,
            product.CategoryId,
            categoryName,
            product.IsActive,
            product.CreatedAt,
            product.UpdatedAt);
}
