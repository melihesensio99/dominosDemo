using MediatR;

namespace Inventory.Api.Features.Stock.Release;

public sealed record ReleaseStockCommand(string ProductId, int Quantity) : IRequest<Result<Inventory.Api.Features.Stock.Common.StockResponse>>;
