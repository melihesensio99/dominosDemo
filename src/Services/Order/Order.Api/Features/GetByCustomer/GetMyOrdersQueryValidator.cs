using FluentValidation;

namespace Order.Api.Features.GetByCustomer;

public sealed class GetMyOrdersQueryValidator : AbstractValidator<GetMyOrdersQuery>
{
    public GetMyOrdersQueryValidator()
    {
        RuleFor(query => query.CustomerId).NotEmpty();
    }
}
