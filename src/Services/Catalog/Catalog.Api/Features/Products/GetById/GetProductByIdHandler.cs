using Catalog.Api.Features.Products.Common;

namespace Catalog.Api.Features.Products;

public sealed class GetProductByIdHandler(IProductRepository productRepository) : IRequestHandler<GetProductByIdQuery, Result<ProductResponse>>
{
    public async Task<Result<ProductResponse>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
        {
            return Result<ProductResponse>.NotFound("catalog.product_not_found", "Product was not found.");
        }

        return Result<ProductResponse>.Success(ProductMapper.ToResponse(product));
    }
}
