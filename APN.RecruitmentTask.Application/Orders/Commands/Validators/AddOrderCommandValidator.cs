using FluentValidation;

namespace APN.RecruitmentTask.Application.Orders.Commands.Validators;

public class AddOrderCommandValidator: AbstractValidator<AddOrderCommand>
{
    public AddOrderCommandValidator()
    {
        RuleFor(x => x.OrderLines).NotEmpty();
        RuleForEach(x => x.OrderLines)
            .NotNull()
            .SetValidator(new OrderLineValidator());
    }
}