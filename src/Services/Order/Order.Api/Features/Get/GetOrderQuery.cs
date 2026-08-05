using BuildingBlocks.Common;
using MediatR;
using Order.Api.Features.Shared;

namespace Order.Api.Features.Get;

public sealed record GetOrderQuery(string Id) : IRequest<Result<OrderResponse>>;
