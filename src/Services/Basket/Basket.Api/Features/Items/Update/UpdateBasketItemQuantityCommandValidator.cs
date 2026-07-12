using FluentValidation;

namespace Basket.Api.Features.Items.Update;

public sealed class UpdateBasketItemQuantityCommandValidator : AbstractValidator<UpdateBasketItemQuantityCommand>
{
    public UpdateBasketItemQuantityCommandValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
