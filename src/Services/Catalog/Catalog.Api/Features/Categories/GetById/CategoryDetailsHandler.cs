using Catalog.Api.Features.Categories.Common;

namespace Catalog.Api.Features.Categories;

public sealed class CategoryDetailsHandler(ICategoryRepository categoryRepository) : IRequestHandler<CategoryDetailsQuery, Result<CategoryResponse>>
{
    public async Task<Result<CategoryResponse>> Handle(CategoryDetailsQuery request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category is null)
        {
            return Result<CategoryResponse>.NotFound("catalog.category_not_found", "Category was not found.");
        }

        return Result<CategoryResponse>.Success(CategoryMapper.ToResponse(category));
    }
}
