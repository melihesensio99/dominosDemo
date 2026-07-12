namespace Catalog.Api.Features.Categories;

public sealed class DeleteCategoryHandler(
    ICategoryRepository categoryRepository,
    IProductRepository productRepository) : IRequestHandler<DeleteCategoryCommand, Result>
{
    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category is null)
        {
            return Result.NotFound("catalog.category_not_found", "Category was not found.");
        }

        var hasProducts = await productRepository.HasProductsInCategoryAsync(category.Id, cancellationToken);
        if (hasProducts)
        {
            return Result.Conflict("catalog.category_has_products", "Category cannot be deleted because products are using it.");
        }

        await categoryRepository.DeleteAsync(category, cancellationToken);
        return Result.Success();
    }
}
