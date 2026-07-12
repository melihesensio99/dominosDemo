using FluentValidation;

namespace Inventory.Api.Features.Stock.Reserve;

public sealed class ReserveStockCommandValidator : AbstractValidator<ReserveStockCommand>
{
    public ReserveStockCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
