namespace Catalog.Api.Features.Products;

public static class UpdateProductEndpoint
{
    public static IEndpointRouteBuilder MapUpdateProductEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/products/{id:guid}", async (Guid id, UpdateProductCommand command, ISender sender, CancellationToken cancellationToken) =>
        {
            var request = command with { Id = id };
            var result = await sender.Send(request, cancellationToken);
            return result.ToHttpResult();
        });

        return app;
    }
}
