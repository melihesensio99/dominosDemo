using BuildingBlocks.Common;
using MediatR;
using Order.Api.Features.Common;

namespace Order.Api.Features.GetByCustomer;

public sealed record GetCustomerOrdersQuery(string CustomerId) : IRequest<Result<IReadOnlyCollection<OrderResponse>>>;
