using Grpc.Core;
using BuildingBlocks.Resilience;
using Inventory.Contracts.Grpc;

namespace Basket.Api.Infrastructure.Clients.Inventory;

public sealed class InventoryGrpcStockClient(InventoryStockService.InventoryStockServiceClient client) : IInventoryStockClient
{
    public async Task<Result<StockSnapshot>> GetStockAsync(string stockKey, CancellationToken cancellationToken)
    {
        try
        {
            var response = await ResilienceExecutor.ExecuteAsync(
                attemptToken => client.GetStockAsync(new GetStockRequest
                {
                    StockKey = stockKey,
                }, cancellationToken: attemptToken).ResponseAsync,
                cancellationToken,
                shouldRetry: exception => exception is RpcException rpcException
                    && rpcException.StatusCode is StatusCode.Unavailable or StatusCode.DeadlineExceeded or StatusCode.Internal);

            return Result<StockSnapshot>.Success(new StockSnapshot(
                response.StockKey,
                response.Available,
                response.Reserved,
                response.ReorderLevel));
        }
        catch (RpcException exception)
        {
            return exception.ToStockResult();
        }
        catch (TimeoutException)
        {
            return Result<StockSnapshot>.Failure("stock.grpc_timeout", "Inventory request timed out.");
        }
    }
}
