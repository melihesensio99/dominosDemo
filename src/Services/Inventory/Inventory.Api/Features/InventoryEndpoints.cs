using Inventory.Api.Features.Stock;

namespace Inventory.Api.Features;

public static class InventoryEndpoints
{
    public static IEndpointRouteBuilder MapInventoryEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapStockEndpoints();
        return app;
    }
}
