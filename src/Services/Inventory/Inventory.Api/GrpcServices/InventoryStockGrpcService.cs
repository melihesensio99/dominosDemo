using Grpc.Core;
using Inventory.Contracts.Grpc;

namespace Inventory.Api.GrpcServices;

public sealed class InventoryStockGrpcService(IStockRepository stockRepository) : InventoryStockService.InventoryStockServiceBase
{
    public override async Task<GetStockResponse> GetStock(GetStockRequest request, ServerCallContext context)
    {
        var productId = request.ProductId.Trim();
        if (string.IsNullOrWhiteSpace(productId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Product id is required."));
        }

        var stockItem = await stockRepository.GetByProductIdAsync(productId, context.CancellationToken);
        if (stockItem is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Stock item not found."));
        }

        return new GetStockResponse
        {
            ProductId = stockItem.ProductId,
            Available = stockItem.Available,
            Reserved = stockItem.Reserved,
            ReorderLevel = stockItem.ReorderLevel,
        };
    }
}
