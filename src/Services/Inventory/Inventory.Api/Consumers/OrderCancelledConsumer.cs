using Inventory.Api.Features.Reservations;
using Inventory.Contracts.IntegrationEvents.Order;
using MassTransit;

namespace Inventory.Api.Consumers;

public sealed class OrderCancelledConsumer(StockReservationService reservationService)
    : IConsumer<OrderCancelledIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderCancelledIntegrationEvent> context)
    {
        var result = await reservationService.ReleaseAsync(context.Message.OrderId, context.CancellationToken);
        if (!result.Success && result.ErrorCode != "inventory.reservation_not_found")
        {
            throw new InvalidOperationException(result.Message);
        }
    }
}
