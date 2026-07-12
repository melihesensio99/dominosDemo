using Inventory.Api.Features.Stock.Common;

namespace Inventory.Api.Features.Stock.GetByProductId;

public sealed class GetStockByProductIdHandler(IStockRepository stockRepository) : IRequestHandler<GetStockByProductIdQuery, Result<StockResponse>>
{
    public async Task<Result<StockResponse>> Handle(GetStockByProductIdQuery request, CancellationToken cancellationToken)
    {
        var stockItem = await stockRepository.GetByProductIdAsync(request.ProductId, cancellationToken);
        if (stockItem is null)
        {
            return Result<StockResponse>.NotFound("inventory.stock_not_found", "Stock item was not found.");
        }

        return Result<StockResponse>.Success(StockMapper.ToResponse(stockItem));
    }
}
