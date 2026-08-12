using BuildingBlocks.Common;
using Order.Api.Abstractions;
using Order.Api.Features.Shared;

namespace Order.Api.Features.GetByCustomer;

public sealed class GetMyOrdersHandler(IOrderRepository orderRepository) : IRequestHandler<GetMyOrdersQuery, Result<IReadOnlyCollection<OrderResponse>>>
{
    public async Task<Result<IReadOnlyCollection<OrderResponse>>> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
        => await CustomerOrdersQueryExecutor.HandleAsync(orderRepository, request.CustomerId, cancellationToken);
}
