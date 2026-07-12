namespace Inventory.Api.Features.Stock.Reserve;

public static class ReserveStockEndpoint
{
    public static IEndpointRouteBuilder MapReserveStockEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/stock/{productId}/reserve", async (string productId, ReserveStockCommand command, ISender sender, CancellationToken cancellationToken) =>
        {
            var request = command with { ProductId = productId };
            var result = await sender.Send(request, cancellationToken);
            return result.ToHttpResult();
        });

        return app;
    }
}
