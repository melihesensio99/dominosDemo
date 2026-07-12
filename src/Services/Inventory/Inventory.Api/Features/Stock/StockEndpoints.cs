using Inventory.Api.Features.Stock.Adjust;
using Inventory.Api.Features.Stock.GetAll;
using Inventory.Api.Features.Stock.GetByProductId;
using Inventory.Api.Features.Stock.Release;
using Inventory.Api.Features.Stock.Reserve;

namespace Inventory.Api.Features.Stock;

public static class StockEndpoints
{
    public static IEndpointRouteBuilder MapStockEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetStocksEndpoint();
        app.MapGetStockByProductIdEndpoint();
        app.MapReserveStockEndpoint();
        app.MapReleaseStockEndpoint();
        app.MapAdjustStockEndpoint();

        return app;
    }
}
