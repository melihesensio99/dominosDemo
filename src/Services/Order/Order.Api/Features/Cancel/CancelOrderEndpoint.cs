using BuildingBlocks.Common;
using MediatR;

namespace Order.Api.Features.Cancel;

public static class CancelOrderEndpoint
{
    public static IEndpointRouteBuilder MapCancelOrderEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/orders/{id}/cancel", async (string id, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new CancelOrderCommand(id), cancellationToken);
            return result.ToHttpResult();
        });

        return app;
    }
}
