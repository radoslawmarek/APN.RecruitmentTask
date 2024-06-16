using APN.RecruitmentTask.Domain.Book;
using FluentValidation;

namespace APN.RecruitmentTask.Application.Books.Commands.Validators;

public class BookAuthorValidator: AbstractValidator<BookAuthor>
{
    public BookAuthorValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
    }
}