using BuildingBlocks.Persistence;
using Microsoft.EntityFrameworkCore;
using Order.Api.Domain;
using Order.Api.Infrastructure.Outbox;
using OrderEntity = Order.Api.Domain.Order;

namespace Order.Api.Infrastructure.Persistence;

public static class OrderDatabaseInitializer
{
    public static async Task InitializeOrderDatabaseAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

        await dbContext.Database.MigrateOrEnsureCreatedAsync(cancellationToken);

        if (await dbContext.Orders.AnyAsync(cancellationToken))
        {
            return;
        }

        var order = OrderEntity.Create(
            "customer-001",
            new[]
            {
                new OrderItem("p-100", 2),
                new OrderItem("p-200", 1),
            },
            Address.Create("Demo Street 1", "Central", "Istanbul", "34000", "Turkey"),
            Address.Create("Demo Billing Street 5", "Central", "Istanbul", "34000", "Turkey"),
            PaymentMethod.Card);

        dbContext.Orders.Add(order);
        dbContext.OutboxMessages.AddRange(OrderOutboxMessageFactory.Create(order.DomainEvents));
        order.ClearDomainEvents();

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
