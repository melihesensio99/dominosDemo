using BuildingBlocks.Common;
using MediatR;
using Order.Api.Features.Common;

namespace Order.Api.Features.Get;

public sealed record GetOrderQuery(string Id) : IRequest<Result<OrderResponse>>;
