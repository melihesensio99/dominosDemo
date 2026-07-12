using BuildingBlocks.Common;

namespace Basket.Api.Features.Get;

public static class GetBasketEndpoint
{
    public static IEndpointRouteBuilder MapGetBasketEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/baskets/{customerId}", async (string customerId, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetBasketQuery(customerId), cancellationToken);
            return result.ToHttpResult();
        });

        return app;
    }
}
