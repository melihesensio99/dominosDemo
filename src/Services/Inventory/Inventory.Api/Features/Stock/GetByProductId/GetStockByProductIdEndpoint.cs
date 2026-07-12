namespace Inventory.Api.Features.Stock.GetByProductId;

public static class GetStockByProductIdEndpoint
{
    public static IEndpointRouteBuilder MapGetStockByProductIdEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/stock/{productId}", async (string productId, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetStockByProductIdQuery(productId), cancellationToken);
            return result.ToHttpResult();
        });

        return app;
    }
}
