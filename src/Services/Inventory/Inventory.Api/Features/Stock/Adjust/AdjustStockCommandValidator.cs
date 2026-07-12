using FluentValidation;

namespace Inventory.Api.Features.Stock.Adjust;

public sealed class AdjustStockCommandValidator : AbstractValidator<AdjustStockCommand>
{
    public AdjustStockCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Quantity).NotEqual(0);
    }
}
