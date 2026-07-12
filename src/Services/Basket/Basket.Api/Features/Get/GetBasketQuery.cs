using BuildingBlocks.Common;

namespace Basket.Api.Features.Get;

public sealed record GetBasketQuery(string CustomerId) : IRequest<Result<BasketResponse>>;
