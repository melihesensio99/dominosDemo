using Catalog.Api.Features.Products.Common;

namespace Catalog.Api.Features.Products;

public sealed class UpdateProductHandler(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository) : IRequestHandler<UpdateProductCommand, Result<ProductResponse>>
{
    public async Task<Result<ProductResponse>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
        {
            return Result<ProductResponse>.NotFound("catalog.product_not_found", "Product was not found.");
        }

        var category = await categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category is null)
        {
            return Result<ProductResponse>.NotFound("catalog.category_not_found", "Category was not found.");
        }

        var existingProduct = await productRepository.GetByNameAsync(request.Name.Trim(), cancellationToken);
        if (existingProduct is not null && existingProduct.Id != product.Id)
        {
            return Result<ProductResponse>.Conflict("catalog.product_exists", "A product with this name already exists.");
        }

        product.Name = request.Name.Trim();
        product.Description = request.Description.Trim();
        product.Price = request.Price;
        product.Stock = request.Stock;
        product.CategoryId = request.CategoryId;
        product.IsActive = request.IsActive;
        product.UpdatedAt = DateTimeOffset.UtcNow;

        await productRepository.UpdateAsync(product, cancellationToken);

        product.Category = category;

        return Result<ProductResponse>.Success(ProductMapper.ToResponse(product));
    }
}
