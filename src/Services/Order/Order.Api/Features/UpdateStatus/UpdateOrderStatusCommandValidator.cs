using FluentValidation;
using Order.Api.Domain;

namespace Order.Api.Features.UpdateStatus;

public sealed class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
{
    public UpdateOrderStatusCommandValidator()
    {
        RuleFor(command => command.OrderId).NotEmpty();
        RuleFor(command => command.Status)
            .NotEmpty()
            .Must(status => Enum.TryParse<OrderStatus>(status, true, out var parsed)
                && parsed != OrderStatus.Pending)
            .WithMessage("Status must be one of: confirmed, preparing, shipped, delivered, cancelled.");
    }
}
