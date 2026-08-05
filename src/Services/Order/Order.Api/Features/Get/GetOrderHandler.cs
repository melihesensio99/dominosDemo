using BuildingBlocks.Common;
using Order.Api.Abstractions;
using Order.Api.Features.Shared;

namespace Order.Api.Features.Get;

public sealed class GetOrderHandler(IOrderRepository orderRepository) : IRequestHandler<GetOrderQuery, Result<OrderResponse>>
{
    public async Task<Result<OrderResponse>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.Id, cancellationToken);
        if (order is null)
        {
            return Result<OrderResponse>.NotFound("order.not_found", "Order was not found.");
        }

        return Result<OrderResponse>.Success(OrderMapper.ToResponse(order));
    }
}
