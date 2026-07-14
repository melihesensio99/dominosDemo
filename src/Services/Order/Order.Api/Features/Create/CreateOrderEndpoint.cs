using BuildingBlocks.Common;
using MediatR;

namespace Order.Api.Features.Create;

public static class CreateOrderEndpoint
{
    public static IEndpointRouteBuilder MapCreateOrderEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/orders", async (CreateOrderCommand command, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);
            return result.ToHttpResult();
        });

        return app;
    }
}
