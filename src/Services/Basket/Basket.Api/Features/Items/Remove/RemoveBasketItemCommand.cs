using BuildingBlocks.Common;

namespace Basket.Api.Features.Items.Remove;

public sealed record RemoveBasketItemCommand(string CustomerId, string ProductId) : IRequest<Result<BasketResponse>>;
