using Order.Api.Features.Cancel;
using Order.Api.Features.Create;
using Order.Api.Features.Get;
using Order.Api.Features.GetByCustomer;
using Order.Api.Features.List;
using Order.Api.Features.UpdateStatus;

namespace Order.Api.Features;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapCreateOrderEndpoint();
        app.MapGetOrdersEndpoint();
        app.MapGetOrderEndpoint();
        app.MapGetMyOrdersEndpoint();
        app.MapGetCustomerOrdersEndpoint();
        app.MapCancelOrderEndpoint();
        app.MapUpdateOrderStatusEndpoint();
        return app;
    }
}
