using BuildingBlocks.Common;
using MediatR;
using Order.Api.Features.Common;

namespace Order.Api.Features.Cancel;

public sealed record CancelOrderCommand(string Id) : IRequest<Result<OrderResponse>>;
