using FluentValidation;

namespace Catalog.Api.Features.Products;

public sealed class CreateProductOptionValidator : AbstractValidator<CreateProductOption>
{
    public CreateProductOptionValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.PriceAdjustment).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DisplayOrder).GreaterThanOrEqualTo(0);
    }
}
