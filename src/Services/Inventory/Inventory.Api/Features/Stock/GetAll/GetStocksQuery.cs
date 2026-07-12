using MediatR;

namespace Inventory.Api.Features.Stock.GetAll;

public sealed record GetStocksQuery : IRequest<Result<IReadOnlyList<Inventory.Api.Features.Stock.Common.StockResponse>>>;
