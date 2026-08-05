using BuildingBlocks.Common;
using Order.Api.Abstractions;
using Order.Api.Features.Shared;

namespace Order.Api.Features.GetByCustomer;

public sealed class GetCustomerOrdersHandler(IOrderRepository orderRepository) : IRequestHandler<GetCustomerOrdersQuery, Result<IReadOnlyCollection<OrderResponse>>>
{
    public async Task<Result<IReadOnlyCollection<OrderResponse>>> Handle(GetCustomerOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await orderRepository.GetByCustomerIdAsync(request.CustomerId, cancellationToken);
        var items = orders.Select(OrderMapper.ToResponse).ToArray();
        return Result<IReadOnlyCollection<OrderResponse>>.Success(items);
    }
}
