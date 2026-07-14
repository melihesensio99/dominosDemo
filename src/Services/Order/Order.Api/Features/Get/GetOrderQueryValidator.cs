using FluentValidation;

namespace Order.Api.Features.Get;

public sealed class GetOrderQueryValidator : AbstractValidator<GetOrderQuery>
{
    public GetOrderQueryValidator()
    {
        RuleFor(query => query.Id).NotEmpty();
    }
}
