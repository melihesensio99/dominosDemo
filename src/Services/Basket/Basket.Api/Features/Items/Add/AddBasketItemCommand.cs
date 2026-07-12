using BuildingBlocks.Common;

namespace Basket.Api.Features.Items.Add;

public sealed record AddBasketItemCommand(string CustomerId, string ProductId, int Quantity) : IRequest<Result<BasketResponse>>;
