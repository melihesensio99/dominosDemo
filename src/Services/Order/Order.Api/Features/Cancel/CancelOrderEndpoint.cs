using BuildingBlocks.Common;
using BuildingBlocks.Security;
using MediatR;
using System.Security.Claims;

namespace Order.Api.Features.Cancel;

public static class CancelOrderEndpoint
{
    public static IEndpointRouteBuilder MapCancelOrderEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/orders/{id}/cancel", async (
            string id,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            if (!user.TryGetUserId(out var customerId))
            {
                return Results.Unauthorized();
            }

            var result = await sender.Send(new CancelOrderCommand(id, customerId), cancellationToken);
            return result.ToHttpResult();
        }).RequireAuthorization();

        return app;
    }
}
