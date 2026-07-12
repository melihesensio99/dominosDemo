namespace Catalog.Api.Features.Categories;

public static class CategoryDetailsEndpoint
{
    public static IEndpointRouteBuilder MapCategoryDetailsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/categories/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new CategoryDetailsQuery(id), cancellationToken);
            return result.ToHttpResult();
        });

        return app;
    }
}
