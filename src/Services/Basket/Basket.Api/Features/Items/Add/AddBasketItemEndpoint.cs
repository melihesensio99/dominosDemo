using BuildingBlocks.Common;
using BuildingBlocks.Security;
using System.Security.Claims;

namespace Basket.Api.Features.Items.Add;

public static class AddBasketItemEndpoint
{
    public static IEndpointRouteBuilder MapAddBasketItemEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/baskets/me/items", async (ClaimsPrincipal user, AddBasketItemCommand command, ISender sender, CancellationToken cancellationToken) =>
        {
            if (!user.TryGetUserId(out var customerId))
            {
                return Results.Unauthorized();
            }

            var result = await sender.Send(command with { CustomerId = customerId }, cancellationToken);
            return result.ToHttpResult();
        }).RequireAuthorization();

        return app;
    }
}
