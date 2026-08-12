using Basket.Api.Features.Clear;
using Basket.Api.Features.Get;
using Basket.Api.Features.Items.Add;
using Basket.Api.Features.Items.Remove;
using Basket.Api.Features.Items.Update;

namespace Basket.Api.Features;

public static class BasketEndpoints
{
    public static IEndpointRouteBuilder MapBasketEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetBasketEndpoint();
        app.MapAddBasketItemEndpoint();
        app.MapUpdateBasketItemQuantityEndpoint();
        app.MapRemoveBasketItemEndpoint();
        app.MapClearBasketEndpoint();
        return app;
    }
}
