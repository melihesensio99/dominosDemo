using Catalog.Api.Features.Products.Common;

namespace Catalog.Api.Features.Products;

public sealed class CreateProductHandler(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository) : IRequestHandler<CreateProductCommand, Result<ProductResponse>>
{
    public async Task<Result<ProductResponse>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category is null)
        {
            return Result<ProductResponse>.NotFound("catalog.category_not_found", "Category was not found.");
        }

        var existingProduct = await productRepository.GetByNameAsync(request.Name.Trim(), cancellationToken);
        if (existingProduct is not null)
        {
            return Result<ProductResponse>.Conflict("catalog.product_exists", "A product with this name already exists.");
        }

        var product = new Product
        {
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            Price = request.Price,
            Stock = request.Stock,
            CategoryId = request.CategoryId,
        };

        await productRepository.AddAsync(product, cancellationToken);

        return Result<ProductResponse>.Success(ProductMapper.ToResponse(product, category.Name));
    }
}
