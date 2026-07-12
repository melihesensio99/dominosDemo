namespace Catalog.Api.Features.Categories;

public static class CreateCategoryEndpoint
{
    public static IEndpointRouteBuilder MapCreateCategoryEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/categories", async (CreateCategoryCommand command, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);
            return result.ToHttpResult();
        });

        return app;
    }
}
