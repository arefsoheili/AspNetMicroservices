using FluentValidation;

namespace Ordering.Application.Features.Orders.Queries.GetOrderById;

public class GetOrderByIdQueryValidator : AbstractValidator<GetOrderByIdQuery>
{
    public GetOrderByIdQueryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
