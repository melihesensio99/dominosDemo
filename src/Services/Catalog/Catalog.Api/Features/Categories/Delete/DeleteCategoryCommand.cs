using MediatR;

namespace Catalog.Api.Features.Categories;

public sealed record DeleteCategoryCommand(Guid Id) : IRequest<Result>;
