using FluentValidation;

namespace Inventory.Api.Features.Stock.Release;

public sealed class ReleaseStockCommandValidator : AbstractValidator<ReleaseStockCommand>
{
    public ReleaseStockCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
