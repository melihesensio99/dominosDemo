namespace Inventory.Api.Features.Stock.GetAll;

public static class GetStocksEndpoint
{
    public static IEndpointRouteBuilder MapGetStocksEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/stock", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetStocksQuery(), cancellationToken);
            return result.ToHttpResult();
        });

        return app;
    }
}
