using Grpc.Core;
using Inventory.Contracts.Grpc;

namespace Basket.Api.Infrastructure;

public sealed class InventoryGrpcStockClient(InventoryStockService.InventoryStockServiceClient client) : IInventoryStockClient
{
    public async Task<Result<StockSnapshot>> GetStockAsync(string productId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await client.GetStockAsync(new GetStockRequest
            {
                ProductId = productId,
            }, cancellationToken: cancellationToken);

            return Result<StockSnapshot>.Success(new StockSnapshot(
                response.ProductId,
                response.Available,
                response.Reserved,
                response.ReorderLevel));
        }
        catch (RpcException exception)
        {
            return exception.ToStockResult();
        }
    }
}
