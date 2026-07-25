using BuildingBlocks.Common;
using BuildingBlocks.Security;
using System.Security.Claims;

namespace Basket.Api.Features.Items.Update;

public static class UpdateBasketItemQuantityEndpoint
{
    public static IEndpointRouteBuilder MapUpdateBasketItemQuantityEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/baskets/me/items/{productId}", async (ClaimsPrincipal user, string productId, UpdateBasketItemQuantityCommand command, ISender sender, CancellationToken cancellationToken) =>
        {
            if (!user.TryGetUserId(out var customerId))
            {
                return Results.Unauthorized();
            }

            var result = await sender.Send(command with { CustomerId = customerId, ProductId = productId }, cancellationToken);
            return result.ToHttpResult();
        }).RequireAuthorization();

        return app;
    }
}
