using BuildingBlocks.Common;

namespace Basket.Api.Features.Items.Update;

public sealed record UpdateBasketItemQuantityCommand(string CustomerId, string ProductId, int Quantity) : IRequest<Result<BasketResponse>>;
