using BuildingBlocks.Common;
using BuildingBlocks.Security;
using MediatR;
using System.Security.Claims;

namespace Order.Api.Features.Create;

public static class CreateOrderEndpoint
{
    public static IEndpointRouteBuilder MapCreateOrderEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/orders", async (CreateOrderRequest request, ClaimsPrincipal user, ISender sender, CancellationToken cancellationToken) =>
        {
            if (!user.TryGetUserId(out var customerId))
            {
                return Results.Unauthorized();
            }

            var command = new CreateOrderCommand(
                customerId,
                request.Items,
                request.ShippingAddress,
                request.BillingAddress,
                request.PaymentMethod);

            var result = await sender.Send(command, cancellationToken);
            return result.ToHttpResult();
        }).RequireAuthorization();

        return app;
    }
}
