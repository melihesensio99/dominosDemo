namespace Catalog.Api.Features.Categories.Common;

public static class CategoryMapper
{
    public static CategoryResponse ToResponse(Category category) =>
        new(category.Id, category.Name, category.Slug, category.IsActive, category.CreatedAt);
}
