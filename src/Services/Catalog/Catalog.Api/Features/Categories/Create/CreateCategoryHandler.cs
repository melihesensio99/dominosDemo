using Catalog.Api.Features.Categories.Common;

namespace Catalog.Api.Features.Categories;

public sealed class CreateCategoryHandler(ICategoryRepository categoryRepository) : IRequestHandler<CreateCategoryCommand, Result<CategoryResponse>>
{
    public async Task<Result<CategoryResponse>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();
        var existingCategory = await categoryRepository.GetByNameAsync(name, cancellationToken);
        var slug = SlugHelper.Slugify(name);
        var existingCategoryBySlug = await categoryRepository.GetBySlugAsync(slug, cancellationToken);

        if (existingCategory is not null || existingCategoryBySlug is not null)
        {
            return Result<CategoryResponse>.Conflict("catalog.category_exists", "A category with this name already exists.");
        }

        var category = new Category
        {
            Name = name,
            Slug = slug,
        };

        await categoryRepository.AddAsync(category, cancellationToken);

        return Result<CategoryResponse>.Success(CategoryMapper.ToResponse(category));
    }
}
