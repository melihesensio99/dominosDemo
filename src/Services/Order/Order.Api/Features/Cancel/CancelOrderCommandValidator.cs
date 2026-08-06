using FluentValidation;

namespace Order.Api.Features.Cancel;

public sealed class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
{
    public CancelOrderCommandValidator()
    {
        RuleFor(command => command.Id).NotEmpty();
        RuleFor(command => command.CustomerId).NotEmpty();
    }
}
