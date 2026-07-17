using BuildingBlocks.Common;
using MediatR;

namespace Order.Api.Features.GetByCustomer;

public static class GetCustomerOrdersEndpoint
{
    public static IEndpointRouteBuilder MapGetCustomerOrdersEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/orders/customers/{customerId}", async (string customerId, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetCustomerOrdersQuery(customerId), cancellationToken);
            return result.ToHttpResult();
        }).RequireAuthorization("AdminOnly");

        return app;
    }
}
