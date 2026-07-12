namespace Inventory.Api.Features.Stock.Adjust;

public static class AdjustStockEndpoint
{
    public static IEndpointRouteBuilder MapAdjustStockEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/stock/{productId}/adjust", async (string productId, AdjustStockCommand command, ISender sender, CancellationToken cancellationToken) =>
        {
            var request = command with { ProductId = productId };
            var result = await sender.Send(request, cancellationToken);
            return result.ToHttpResult();
        });

        return app;
    }
}
