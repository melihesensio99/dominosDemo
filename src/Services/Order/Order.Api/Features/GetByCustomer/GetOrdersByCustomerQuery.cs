using BuildingBlocks.Common;
using MediatR;
using Order.Api.Features.Shared;

namespace Order.Api.Features.GetByCustomer;

public sealed record GetMyOrdersQuery(string CustomerId) : IRequest<Result<IReadOnlyCollection<OrderResponse>>>;
