using FluentValidation;

namespace Catalog.Api.Features.Products;

public sealed class CreateProductOptionGroupValidator : AbstractValidator<CreateProductOptionGroup>
{
    public CreateProductOptionGroupValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.SelectionType)
            .Must(value => value is "single" or "multiple")
            .WithMessage("SelectionType must be single or multiple.");
        RuleFor(x => x.Options).NotEmpty();
        RuleForEach(x => x.Options).SetValidator(new CreateProductOptionValidator());
    }
}
