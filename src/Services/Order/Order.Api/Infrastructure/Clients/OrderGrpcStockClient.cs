using Grpc.Core;
using Inventory.Contracts.Grpc;
using Order.Api.Abstractions;

namespace Order.Api.Infrastructure.Clients;

public sealed class OrderGrpcStockClient(InventoryStockService.InventoryStockServiceClient client)
    : IOrderStockClient
{
    public async Task<OrderStockResult> ReserveAsync(
        string orderId,
        IReadOnlyCollection<OrderStockRequirement> requirements,
        CancellationToken cancellationToken)
    {
        var request = new ReserveOrderStockRequest { OrderId = orderId };
        request.Items.AddRange(requirements.Select(item => new StockRequirement
        {
            StockKey = item.StockKey,
            Quantity = item.Quantity,
        }));

        try
        {
            return Map(await client.ReserveOrderStockAsync(request, cancellationToken: cancellationToken));
        }
        catch (RpcException exception)
        {
            return new OrderStockResult(false, "inventory.unavailable", exception.Status.Detail);
        }
    }

    public async Task<OrderStockResult> ReleaseAsync(string orderId, CancellationToken cancellationToken)
    {
        try
        {
            return Map(await client.ReleaseOrderStockAsync(
                new OrderStockReservationRequest { OrderId = orderId },
                cancellationToken: cancellationToken));
        }
        catch (RpcException exception)
        {
            return new OrderStockResult(false, "inventory.unavailable", exception.Status.Detail);
        }
    }

    private static OrderStockResult Map(StockReservationResponse response) =>
        new(response.Success, response.ErrorCode, response.Message);
}
