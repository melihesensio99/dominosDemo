using BuildingBlocks.Common;
using MediatR;

namespace Order.Api.Features.List;

public static class ListOrdersEndpoint
{
    public static IEndpointRouteBuilder MapGetOrdersEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/orders", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new ListOrdersQuery(), cancellationToken);
            return result.ToHttpResult();
        });

        return app;
    }
}
