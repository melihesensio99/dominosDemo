using BuildingBlocks.Common;

namespace Basket.Api.Features.Items.Add;

public static class AddBasketItemEndpoint
{
    public static IEndpointRouteBuilder MapAddBasketItemEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/baskets/{customerId}/items", async (string customerId, AddBasketItemCommand command, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command with { CustomerId = customerId }, cancellationToken);
            return result.ToHttpResult();
        });

        return app;
    }
}
