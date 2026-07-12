using MediatR;

namespace Inventory.Api.Features.Stock.Adjust;

public sealed record AdjustStockCommand(string ProductId, int Quantity) : IRequest<Result<Inventory.Api.Features.Stock.Common.StockResponse>>;
