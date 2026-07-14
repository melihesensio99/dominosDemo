using BuildingBlocks.Common;
using MediatR;
using Order.Api.Features.Common;

namespace Order.Api.Features.List;

public sealed record ListOrdersQuery() : IRequest<Result<IReadOnlyCollection<OrderResponse>>>;
