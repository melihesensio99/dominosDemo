using BuildingBlocks.Common;
using BuildingBlocks.Security;
using System.Security.Claims;

namespace Basket.Api.Features.Items.Update;

public static class UpdateBasketItemQuantityEndpoint
{
    public static IEndpointRouteBuilder MapUpdateBasketItemQuantityEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/baskets/me/items/{itemId}", async (ClaimsPrincipal user, Guid itemId, UpdateBasketItemQuantityCommand command, ISender sender, CancellationToken cancellationToken) =>
        {
            if (!user.TryGetUserId(out var customerId))
            {
                return Results.Unauthorized();
            }

            var result = await sender.Send(command with { CustomerId = customerId, ItemId = itemId }, cancellationToken);
            return result.ToHttpResult();
        }).RequireAuthorization();

        return app;
    }
}
