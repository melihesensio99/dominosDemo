using BuildingBlocks.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Order.Api.Features.Get;

public static class GetOrderEndpoint
{
    public static IEndpointRouteBuilder MapGetOrderEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/orders/{id}", async (string id, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetOrderQuery(id), cancellationToken);
            return result.ToHttpResult();
        }).RequireAuthorization("AdminOnly");

        return app;
    }
}
