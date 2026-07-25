using BuildingBlocks.Common;
using BuildingBlocks.Security;
using System.Security.Claims;

namespace Basket.Api.Features.Get;

public static class GetBasketEndpoint
{
    public static IEndpointRouteBuilder MapGetBasketEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/baskets/me", async (ClaimsPrincipal user, ISender sender, CancellationToken cancellationToken) =>
        {
            if (!user.TryGetUserId(out var customerId))
            {
                return Results.Unauthorized();
            }

            var result = await sender.Send(new GetBasketQuery(customerId), cancellationToken);
            return result.ToHttpResult();
        }).RequireAuthorization();

        return app;
    }
}
