using BuildingBlocks.Common;

namespace Basket.Api.Features.Items.Add;

public sealed record AddBasketItemCommand(
    string CustomerId,
    string ProductId,
    int Quantity,
    IReadOnlyList<Guid>? SelectedOptionIds = null) : IRequest<Result<BasketResponse>>;
