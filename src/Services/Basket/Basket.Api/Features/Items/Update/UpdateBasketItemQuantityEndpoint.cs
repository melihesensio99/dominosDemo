using BuildingBlocks.Common;

namespace Basket.Api.Features.Items.Update;

public static class UpdateBasketItemQuantityEndpoint
{
    public static IEndpointRouteBuilder MapUpdateBasketItemQuantityEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/baskets/{customerId}/items/{productId}", async (string customerId, string productId, UpdateBasketItemQuantityCommand command, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command with { CustomerId = customerId, ProductId = productId }, cancellationToken);
            return result.ToHttpResult();
        });

        return app;
    }
}
