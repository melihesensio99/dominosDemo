using Grpc.Core;
using Grpc.Net.Client;
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
        catch (RpcException exception) when (exception.StatusCode == StatusCode.NotFound)
        {
            return Result<StockSnapshot>.NotFound("stock.not_found", "The requested product stock was not found.");
        }
        catch (RpcException exception) when (exception.StatusCode == StatusCode.InvalidArgument)
        {
            return Result<StockSnapshot>.Validation("stock.invalid_product", exception.Status.Detail);
        }
        catch (RpcException exception)
        {
            return Result<StockSnapshot>.Failure("stock.grpc_error", exception.Status.Detail);
        }
    }

    public async Task<bool> IsReadyAsync(CancellationToken cancellationToken)
    {
        try
        {
            _ = await client.GetStockAsync(new GetStockRequest
            {
                ProductId = "__healthcheck__",
            }, cancellationToken: cancellationToken);

            return true;
        }
        catch (RpcException exception) when (exception.StatusCode is StatusCode.NotFound or StatusCode.InvalidArgument)
        {
            return true;
        }
        catch
        {
            return false;
        }
    }
}
