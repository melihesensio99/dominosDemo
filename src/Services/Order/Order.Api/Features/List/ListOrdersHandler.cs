using BuildingBlocks.Common;
using Order.Api.Abstractions;
using Order.Api.Features.Shared;

namespace Order.Api.Features.List;

public sealed class ListOrdersHandler(IOrderRepository orderRepository) : IRequestHandler<ListOrdersQuery, Result<IReadOnlyCollection<OrderResponse>>>
{
    public async Task<Result<IReadOnlyCollection<OrderResponse>>> Handle(ListOrdersQuery request, CancellationToken cancellationToken)
    {
        _ = request;

        var orders = await orderRepository.GetAllAsync(cancellationToken);
        var items = orders.Select(OrderMapper.ToResponse).ToArray();
        return Result<IReadOnlyCollection<OrderResponse>>.Success(items);
    }
}
