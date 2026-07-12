using FluentValidation;

namespace Basket.Api.Features.Clear;

public sealed class ClearBasketCommandValidator : AbstractValidator<ClearBasketCommand>
{
    public ClearBasketCommandValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
    }
}
