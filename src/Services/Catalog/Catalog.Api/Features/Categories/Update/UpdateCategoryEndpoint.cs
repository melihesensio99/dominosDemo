namespace Catalog.Api.Features.Categories;

public static class UpdateCategoryEndpoint
{
    public static IEndpointRouteBuilder MapUpdateCategoryEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/categories/{id:guid}", async (Guid id, UpdateCategoryCommand command, ISender sender, CancellationToken cancellationToken) =>
        {
            var request = command with { Id = id };
            var result = await sender.Send(request, cancellationToken);
            return result.ToHttpResult();
        });

        return app;
    }
}
