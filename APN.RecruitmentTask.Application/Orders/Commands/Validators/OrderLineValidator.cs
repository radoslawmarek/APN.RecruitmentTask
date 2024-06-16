using APN.RecruitmentTask.Domain.Order;
using FluentValidation;

namespace APN.RecruitmentTask.Application.Orders.Commands.Validators;

public class OrderLineValidator: AbstractValidator<OrderLine>
{
    public OrderLineValidator()
    {
        RuleFor(x => x.BookId)
            .NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}