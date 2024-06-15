using FluentValidation;

namespace APN.RecruitmentTask.Application.Books.Commands;

public class AddBookCommandValidator: AbstractValidator<AddBookCommand>
{
    public AddBookCommandValidator()
    {
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.BookStand).GreaterThan(0);
        RuleFor(x => x.Shelf).GreaterThan(0);
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Authors).NotEmpty();
        RuleForEach(x => x.Authors)
            .NotNull()
            .SetValidator(new BookAuthorValidator());
    }
}