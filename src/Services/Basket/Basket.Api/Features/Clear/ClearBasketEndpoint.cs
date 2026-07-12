using BuildingBlocks.Common;

namespace Basket.Api.Features.Clear;

public static class ClearBasketEndpoint
{
    public static IEndpointRouteBuilder MapClearBasketEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/baskets/{customerId}", async (string customerId, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new ClearBasketCommand(customerId), cancellationToken);
            return result.ToHttpResult();
        });

        return app;
    }
}
