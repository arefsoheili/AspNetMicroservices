using FluentValidation;

namespace Ordering.Application.Features.Orders.Commands.UpdateOrder;

public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderCommandValidator()
    {
        RuleFor(p => p.UserName)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .MaximumLength(50);

        RuleFor(p => p.EmailAddress)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .EmailAddress();

        RuleFor(p => p.TotalPrice)
            .GreaterThan(0).WithMessage("{PropertyName} should be greater than zero.");
    }
}
