using Catalog.Api.Features.Categories.Common;

namespace Catalog.Api.Features.Categories;

public sealed class GetCategoriesHandler(ICategoryRepository categoryRepository) : IRequestHandler<GetCategoriesQuery, Result<IReadOnlyList<CategoryResponse>>>
{
    public async Task<Result<IReadOnlyList<CategoryResponse>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await categoryRepository.GetAllAsync(cancellationToken);
        var response = categories.Select(CategoryMapper.ToResponse).ToList();

        return Result<IReadOnlyList<CategoryResponse>>.Success(response);
    }
}
