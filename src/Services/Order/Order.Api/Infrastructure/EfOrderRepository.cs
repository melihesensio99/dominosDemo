using Microsoft.EntityFrameworkCore;
using Order.Api.Domain.Events;
using Order.Api.Abstractions;
using OrderEntity = Order.Api.Domain.Order;

namespace Order.Api.Infrastructure;

public sealed class EfOrderRepository(OrderDbContext dbContext) : IOrderRepository
{
    public async Task<IReadOnlyCollection<OrderEntity>> GetAllAsync(CancellationToken cancellationToken) =>
        await dbContext.Orders
            .Include(order => order.Items)
            .OrderByDescending(order => order.CreatedAt)
            .ToArrayAsync(cancellationToken);

    public async Task<OrderEntity?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await dbContext.Orders
            .Include(order => order.Items)
            .FirstOrDefaultAsync(order => order.Id == id, cancellationToken);

    public async Task SaveAsync(OrderEntity order, CancellationToken cancellationToken)
    {
        if (dbContext.Entry(order).State == EntityState.Detached)
        {
            dbContext.Orders.Add(order);
        }

        dbContext.OutboxMessages.AddRange(OrderOutboxMessageFactory.Create(order.DomainEvents));
        await dbContext.SaveChangesAsync(cancellationToken);
        order.ClearDomainEvents();
    }
}
