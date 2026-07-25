using FluentValidation;

namespace Catalog.Api.Features.Products;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(250);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Stock).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ReorderLevel).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.ImageUrl).MaximumLength(1000).When(x => !string.IsNullOrWhiteSpace(x.ImageUrl));
        RuleForEach(x => x.OptionGroups).SetValidator(new CreateProductOptionGroupValidator());
    }
}
