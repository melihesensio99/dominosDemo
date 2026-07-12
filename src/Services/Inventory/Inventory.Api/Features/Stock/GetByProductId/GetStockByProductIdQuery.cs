using MediatR;

namespace Inventory.Api.Features.Stock.GetByProductId;

public sealed record GetStockByProductIdQuery(string ProductId) : IRequest<Result<Inventory.Api.Features.Stock.Common.StockResponse>>;
