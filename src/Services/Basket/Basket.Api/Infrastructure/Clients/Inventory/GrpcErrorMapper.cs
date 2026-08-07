using Grpc.Core;

namespace Basket.Api.Infrastructure.Clients.Inventory;

public static class GrpcErrorMapper
{
    public static Result<StockSnapshot> ToStockResult(this RpcException exception)
    {
        return exception.StatusCode switch
        {
            StatusCode.NotFound => Result<StockSnapshot>.NotFound("stock.not_found", "The requested product stock was not found."),
            StatusCode.InvalidArgument => Result<StockSnapshot>.Validation("stock.invalid_product", exception.Status.Detail),
            _ => Result<StockSnapshot>.Failure("stock.grpc_error", exception.Status.Detail),
        };
    }
}
