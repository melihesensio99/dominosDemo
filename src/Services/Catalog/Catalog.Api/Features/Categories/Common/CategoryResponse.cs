namespace Catalog.Api.Features.Categories;

public sealed record CategoryResponse(
    Guid Id,
    string Name,
    string Slug,
    bool IsActive,
    DateTimeOffset CreatedAt);
