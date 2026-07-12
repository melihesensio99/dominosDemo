namespace Catalog.Api.Features.Categories;

public static class GetCategoriesEndpoint
{
    public static IEndpointRouteBuilder MapGetCategoriesEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/categories", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetCategoriesQuery(), cancellationToken);
            return result.ToHttpResult();
        });

        return app;
    }
}
