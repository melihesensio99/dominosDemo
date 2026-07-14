using BuildingBlocks.Common;
using MediatR;

namespace Order.Api.Features.Get;

public static class GetOrderEndpoint
{
    public static IEndpointRouteBuilder MapGetOrderEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/orders/{id}", async (string id, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetOrderQuery(id), cancellationToken);
            return result.ToHttpResult();
        });

        return app;
    }
}
