namespace Catalog.Api.Features.Categories;

public static class DeleteCategoryEndpoint
{
    public static IEndpointRouteBuilder MapDeleteCategoryEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/categories/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new DeleteCategoryCommand(id), cancellationToken);
            return result.ToHttpResult();
        });

        return app;
    }
}
