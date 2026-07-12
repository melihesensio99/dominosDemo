namespace Inventory.Api.Features.Stock.Release;

public static class ReleaseStockEndpoint
{
    public static IEndpointRouteBuilder MapReleaseStockEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/stock/{productId}/release", async (string productId, ReleaseStockCommand command, ISender sender, CancellationToken cancellationToken) =>
        {
            var request = command with { ProductId = productId };
            var result = await sender.Send(request, cancellationToken);
            return result.ToHttpResult();
        });

        return app;
    }
}
