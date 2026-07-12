using BuildingBlocks.Common;

namespace Basket.Api.Features.Clear;

public sealed class ClearBasketHandler(IBasketRepository basketRepository) : IRequestHandler<ClearBasketCommand, Result>
{
    public async Task<Result> Handle(ClearBasketCommand request, CancellationToken cancellationToken)
    {
        await basketRepository.DeleteAsync(request.CustomerId, cancellationToken);
        return Result.Success();
    }
}
