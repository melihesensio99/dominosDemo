using BuildingBlocks.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Order.Api.Features.GetByCustomer;

public static class GetMyOrdersEndpoint
{
    public static IEndpointRouteBuilder MapGetMyOrdersEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/orders/me", async (ClaimsPrincipal user, ISender sender, CancellationToken cancellationToken) =>
        {
            var customerId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? user.FindFirstValue("sub");
            if (string.IsNullOrWhiteSpace(customerId))
            {
                return Results.Unauthorized();
            }

            var result = await sender.Send(new GetMyOrdersQuery(customerId), cancellationToken);
            return result.ToHttpResult();
        }).RequireAuthorization();

        return app;
    }
}
