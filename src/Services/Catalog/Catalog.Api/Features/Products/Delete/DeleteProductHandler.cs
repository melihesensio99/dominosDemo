namespace Catalog.Api.Features.Products;

public sealed class DeleteProductHandler(IProductRepository productRepository) : IRequestHandler<DeleteProductCommand, Result>
{
    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
        {
            return Result.NotFound("catalog.product_not_found", "Product was not found.");
        }

        await productRepository.DeleteWithInventorySyncAsync(product, cancellationToken);
        return Result.Success();
    }
}
