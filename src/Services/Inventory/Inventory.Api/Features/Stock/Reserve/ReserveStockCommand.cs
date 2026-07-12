using MediatR;

namespace Inventory.Api.Features.Stock.Reserve;

public sealed record ReserveStockCommand(string ProductId, int Quantity) : IRequest<Result<Inventory.Api.Features.Stock.Common.StockResponse>>;
