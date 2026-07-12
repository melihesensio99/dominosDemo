using BuildingBlocks.Common;

namespace Basket.Api.Features.Items.Remove;

public static class RemoveBasketItemEndpoint
{
    public static IEndpointRouteBuilder MapRemoveBasketItemEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/baskets/{customerId}/items/{productId}", async (string customerId, string productId, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new RemoveBasketItemCommand(customerId, productId), cancellationToken);
            return result.ToHttpResult();
        });

        return app;
    }
}
