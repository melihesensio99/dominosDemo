using Catalog.Api.Features.Categories.Common;
using Catalog.Api.Features.Common;

namespace Catalog.Api.Features.Categories;

public sealed class UpdateCategoryHandler(ICategoryRepository categoryRepository) : IRequestHandler<UpdateCategoryCommand, Result<CategoryResponse>>
{
    public async Task<Result<CategoryResponse>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category is null)
        {
            return Result<CategoryResponse>.NotFound("catalog.category_not_found", "Category was not found.");
        }

        var name = request.Name.Trim();
        var existingCategory = await categoryRepository.GetByNameAsync(name, cancellationToken);
        var slug = SlugHelper.Slugify(name);
        var existingCategoryBySlug = await categoryRepository.GetBySlugAsync(slug, cancellationToken);

        if ((existingCategory is not null && existingCategory.Id != category.Id) ||
            (existingCategoryBySlug is not null && existingCategoryBySlug.Id != category.Id))
        {
            return Result<CategoryResponse>.Conflict("catalog.category_exists", "A category with this name already exists.");
        }

        category.Name = name;
        category.Slug = slug;
        category.IsActive = request.IsActive;

        await categoryRepository.UpdateAsync(category, cancellationToken);

        return Result<CategoryResponse>.Success(CategoryMapper.ToResponse(category));
    }
}
