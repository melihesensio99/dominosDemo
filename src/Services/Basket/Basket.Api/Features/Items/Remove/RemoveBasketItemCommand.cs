using BuildingBlocks.Common;

namespace Basket.Api.Features.Items.Remove;

public sealed record RemoveBasketItemCommand(string CustomerId, Guid ItemId) : IRequest<Result<BasketResponse>>;
