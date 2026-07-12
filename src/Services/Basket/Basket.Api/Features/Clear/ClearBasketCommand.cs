using BuildingBlocks.Common;

namespace Basket.Api.Features.Clear;

public sealed record ClearBasketCommand(string CustomerId) : IRequest<Result>;
