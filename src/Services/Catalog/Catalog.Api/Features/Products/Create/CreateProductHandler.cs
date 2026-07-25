using Catalog.Api.Features.Products.Common;
using Inventory.Contracts.IntegrationEvents.Catalog;
using MassTransit;

namespace Catalog.Api.Features.Products;

public sealed class CreateProductHandler(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    IPublishEndpoint publishEndpoint) : IRequestHandler<CreateProductCommand, Result<ProductResponse>>
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
            ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? null : request.ImageUrl.Trim(),
            Price = request.Price,
            Stock = request.Stock,
            CategoryId = request.CategoryId,
            OptionGroups = request.OptionGroups?
                .Select(group => new ProductOptionGroup
                {
                    Name = group.Name.Trim(),
                    SelectionType = group.SelectionType.Trim().ToLowerInvariant(),
                    IsRequired = group.IsRequired,
                    DisplayOrder = group.DisplayOrder,
                    Options = group.Options
                        .Select(option => new ProductOption
                        {
                            Name = option.Name.Trim(),
                            PriceAdjustment = option.PriceAdjustment,
                            DisplayOrder = option.DisplayOrder,
                        })
                        .ToList(),
                })
                .ToList() ?? [],
        };

        await productRepository.AddAsync(product, cancellationToken);

        await publishEndpoint.Publish(
            new ProductCreatedIntegrationEvent(
                product.Id.ToString(),
                product.Stock,
                request.ReorderLevel),
            cancellationToken);

        return Result<ProductResponse>.Success(ProductMapper.ToResponse(product, category.Name));
    }
}
