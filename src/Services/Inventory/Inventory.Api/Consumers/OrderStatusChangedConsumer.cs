using Inventory.Api.Features.Reservations;
using Inventory.Contracts.IntegrationEvents.Order;
using MassTransit;

namespace Inventory.Api.Consumers;

public sealed class OrderStatusChangedConsumer(StockReservationService reservationService)
    : IConsumer<OrderStatusChangedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderStatusChangedIntegrationEvent> context)
    {
        if (!context.Message.Status.Equals("delivered", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var result = await reservationService.CommitAsync(context.Message.OrderId, context.CancellationToken);
        if (!result.Success && result.ErrorCode != "inventory.reservation_not_found")
        {
            throw new InvalidOperationException(result.Message);
        }
    }
}
