using Inventory.Api.Features.Stock.Common;

namespace Inventory.Api.Features.Stock.GetAll;

public sealed class GetStocksHandler(IStockRepository stockRepository) : IRequestHandler<GetStocksQuery, Result<IReadOnlyList<StockResponse>>>
{
    public async Task<Result<IReadOnlyList<StockResponse>>> Handle(GetStocksQuery request, CancellationToken cancellationToken)
    {
        var stocks = await stockRepository.GetAllAsync(cancellationToken);
        var response = stocks.Select(StockMapper.ToResponse).ToList();

        return Result<IReadOnlyList<StockResponse>>.Success(response);
    }
}
