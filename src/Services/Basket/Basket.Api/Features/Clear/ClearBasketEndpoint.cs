using BuildingBlocks.Common;
using BuildingBlocks.Security;
using System.Security.Claims;

namespace Basket.Api.Features.Clear;

public static class ClearBasketEndpoint
{
    public static IEndpointRouteBuilder MapClearBasketEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/baskets/me", async (ClaimsPrincipal user, ISender sender, CancellationToken cancellationToken) =>
        {
            if (!user.TryGetUserId(out var customerId))
            {
                return Results.Unauthorized();
            }

            var result = await sender.Send(new ClearBasketCommand(customerId), cancellationToken);
            return result.ToHttpResult();
        }).RequireAuthorization();

        return app;
    }
}
