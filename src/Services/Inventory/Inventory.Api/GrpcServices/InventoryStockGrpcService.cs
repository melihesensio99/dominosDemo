using Grpc.Core;
using Inventory.Api.Features.Reservations;
using Inventory.Contracts.Grpc;

namespace Inventory.Api.GrpcServices;

public sealed class InventoryStockGrpcService(
    IStockRepository stockRepository,
    StockReservationService reservationService) : InventoryStockService.InventoryStockServiceBase
{
    public override async Task<GetStockResponse> GetStock(GetStockRequest request, ServerCallContext context)
    {
        var stockKey = request.StockKey.Trim();
        if (string.IsNullOrWhiteSpace(stockKey))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Stock key is required."));
        }

        var stockItem = await stockRepository.GetByStockKeyAsync(stockKey, context.CancellationToken);
        if (stockItem is null || !stockItem.IsActive)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Stock item not found."));
        }

        return new GetStockResponse
        {
            StockKey = stockItem.StockKey,
            DisplayName = stockItem.DisplayName,
            Available = stockItem.Available,
            Reserved = stockItem.Reserved,
            ReorderLevel = stockItem.ReorderLevel,
        };
    }

    public override async Task<StockReservationResponse> ReserveOrderStock(
        ReserveOrderStockRequest request,
        ServerCallContext context)
    {
        var result = await reservationService.ReserveAsync(
            request.OrderId,
            request.Items.Select(item => new ReservedStockItem(item.StockKey, item.Quantity)),
            context.CancellationToken);
        return Map(result);
    }

    public override async Task<StockReservationResponse> ReleaseOrderStock(
        OrderStockReservationRequest request,
        ServerCallContext context) =>
        Map(await reservationService.ReleaseAsync(request.OrderId, context.CancellationToken));

    public override async Task<StockReservationResponse> CommitOrderStock(
        OrderStockReservationRequest request,
        ServerCallContext context) =>
        Map(await reservationService.CommitAsync(request.OrderId, context.CancellationToken));

    private static StockReservationResponse Map(StockReservationResult result) => new()
    {
        Success = result.Success,
        ErrorCode = result.ErrorCode ?? string.Empty,
        Message = result.Message ?? string.Empty,
    };
}
