using BuildingBlocks.Common;
using Order.Api.Abstractions;
using Order.Api.Features.Shared;

namespace Order.Api.Features.GetByCustomer;

internal static class CustomerOrdersQueryExecutor
{
    public static async Task<Result<IReadOnlyCollection<OrderResponse>>> HandleAsync(
        IOrderRepository orderRepository,
        string customerId,
        CancellationToken cancellationToken)
    {
        var orders = await orderRepository.GetByCustomerIdAsync(customerId, cancellationToken);
        var items = orders.Select(OrderMapper.ToResponse).ToArray();
        return Result<IReadOnlyCollection<OrderResponse>>.Success(items);
    }
}
