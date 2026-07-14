using FluentValidation;

namespace Order.Api.Features.Create;

public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(command => command.CustomerId).NotEmpty();
        RuleFor(command => command.Items).NotEmpty();
        RuleFor(command => command.ShippingAddress).NotNull();
        RuleFor(command => command.BillingAddress).NotNull();
        RuleForEach(command => command.Items).ChildRules(items =>
        {
            items.RuleFor(item => item.ProductId).NotEmpty();
            items.RuleFor(item => item.Quantity).GreaterThan(0);
        });

        RuleFor(command => command.ShippingAddress.Street).NotEmpty();
        RuleFor(command => command.ShippingAddress.District).NotEmpty();
        RuleFor(command => command.ShippingAddress.City).NotEmpty();
        RuleFor(command => command.ShippingAddress.PostalCode).NotEmpty();
        RuleFor(command => command.ShippingAddress.Country).NotEmpty();

        RuleFor(command => command.BillingAddress.Street).NotEmpty();
        RuleFor(command => command.BillingAddress.District).NotEmpty();
        RuleFor(command => command.BillingAddress.City).NotEmpty();
        RuleFor(command => command.BillingAddress.PostalCode).NotEmpty();
        RuleFor(command => command.BillingAddress.Country).NotEmpty();
    }
}
