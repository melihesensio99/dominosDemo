using Grpc.Core;
using BuildingBlocks.Resilience;
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
            var response = await ResilienceExecutor.ExecuteAsync(
                async attemptToken => await client.ReserveOrderStockAsync(request, cancellationToken: attemptToken).ResponseAsync,
                cancellationToken,
                shouldRetry: exception => exception is RpcException rpcException
                    && rpcException.StatusCode is StatusCode.Unavailable or StatusCode.DeadlineExceeded or StatusCode.Internal,
                timeout: TimeSpan.FromSeconds(10));

            return Map(response);
        }
        catch (RpcException exception)
        {
            return new OrderStockResult(false, "inventory.unavailable", exception.Status.Detail);
        }
        catch (TimeoutException)
        {
            return new OrderStockResult(false, "inventory.timeout", "Inventory request timed out.");
        }
    }

    public async Task<OrderStockResult> ReleaseAsync(string orderId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await ResilienceExecutor.ExecuteAsync(
                async attemptToken => await client.ReleaseOrderStockAsync(
                    new OrderStockReservationRequest { OrderId = orderId },
                    cancellationToken: attemptToken).ResponseAsync,
                cancellationToken,
                shouldRetry: exception => exception is RpcException rpcException
                    && rpcException.StatusCode is StatusCode.Unavailable or StatusCode.DeadlineExceeded or StatusCode.Internal,
                timeout: TimeSpan.FromSeconds(10));

            return Map(response);
        }
        catch (RpcException exception)
        {
            return new OrderStockResult(false, "inventory.unavailable", exception.Status.Detail);
        }
        catch (TimeoutException)
        {
            return new OrderStockResult(false, "inventory.timeout", "Inventory request timed out.");
        }
    }

    private static OrderStockResult Map(StockReservationResponse response) =>
        new(response.Success, response.ErrorCode, response.Message);
}
