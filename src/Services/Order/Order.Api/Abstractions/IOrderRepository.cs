using OrderEntity = Order.Api.Domain.Order;

namespace Order.Api.Abstractions;

public interface IOrderRepository
{
    Task<IReadOnlyCollection<OrderEntity>> GetAllAsync(CancellationToken cancellationToken);

    Task<OrderEntity?> GetByIdAsync(string id, CancellationToken cancellationToken);

    Task SaveAsync(OrderEntity order, CancellationToken cancellationToken);
}
