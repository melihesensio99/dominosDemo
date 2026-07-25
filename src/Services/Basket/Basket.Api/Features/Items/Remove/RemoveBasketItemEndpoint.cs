using BuildingBlocks.Common;
using BuildingBlocks.Security;
using System.Security.Claims;

namespace Basket.Api.Features.Items.Remove;

public static class RemoveBasketItemEndpoint
{
    public static IEndpointRouteBuilder MapRemoveBasketItemEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/baskets/me/items/{itemId}", async (ClaimsPrincipal user, Guid itemId, ISender sender, CancellationToken cancellationToken) =>
        {
            if (!user.TryGetUserId(out var customerId))
            {
                return Results.Unauthorized();
            }

            var result = await sender.Send(new RemoveBasketItemCommand(customerId, itemId), cancellationToken);
            return result.ToHttpResult();
        }).RequireAuthorization();

        return app;
    }
}
