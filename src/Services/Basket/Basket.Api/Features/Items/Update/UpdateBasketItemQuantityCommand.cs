using BuildingBlocks.Common;

namespace Basket.Api.Features.Items.Update;

public sealed record UpdateBasketItemQuantityCommand(string CustomerId, Guid ItemId, int Quantity) : IRequest<Result<BasketResponse>>;
