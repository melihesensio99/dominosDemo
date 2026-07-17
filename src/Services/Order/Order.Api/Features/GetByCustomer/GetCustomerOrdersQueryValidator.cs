using FluentValidation;

namespace Order.Api.Features.GetByCustomer;

public sealed class GetCustomerOrdersQueryValidator : AbstractValidator<GetCustomerOrdersQuery>
{
    public GetCustomerOrdersQueryValidator()
    {
        RuleFor(query => query.CustomerId).NotEmpty();
    }
}
