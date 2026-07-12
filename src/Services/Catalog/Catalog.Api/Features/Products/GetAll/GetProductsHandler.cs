using Catalog.Api.Features.Products.Common;

namespace Catalog.Api.Features.Products;

public sealed class GetProductsHandler(IProductRepository productRepository) : IRequestHandler<GetProductsQuery, Result<IReadOnlyList<ProductResponse>>>
{
    public async Task<Result<IReadOnlyList<ProductResponse>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await productRepository.GetAllAsync(cancellationToken);
        var response = products.Select(product => ProductMapper.ToResponse(product)).ToList();

        return Result<IReadOnlyList<ProductResponse>>.Success(response);
    }
}
