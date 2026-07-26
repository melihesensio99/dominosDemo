namespace Order.Api.Features.UpdateStatus;

public static class UpdateOrderStatusEndpoint
{
    public static IEndpointRouteBuilder MapUpdateOrderStatusEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPatch("/admin/orders/{orderId}/status", async (
            string orderId,
            UpdateOrderStatusRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(
                new UpdateOrderStatusCommand(orderId, request.Status),
                cancellationToken);

            return result.ToHttpResult();
        }).RequireAuthorization("AdminOnly");

        return app;
    }

    public sealed record UpdateOrderStatusRequest(string Status);
}
