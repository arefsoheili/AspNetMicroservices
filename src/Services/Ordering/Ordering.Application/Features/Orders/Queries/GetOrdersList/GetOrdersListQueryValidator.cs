using FluentValidation;

namespace Ordering.Application.Features.Orders.Queries.GetOrdersList;

public class GetOrdersListQueryValidator : AbstractValidator<GetOrdersListQuery>
{
    public GetOrdersListQueryValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .MaximumLength(50);
    }
}
