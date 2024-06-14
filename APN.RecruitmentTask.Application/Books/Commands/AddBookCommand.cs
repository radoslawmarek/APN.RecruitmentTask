using APN.RecruitmentTask.Domain.Book;
using ErrorOr;
using MediatR;

namespace APN.RecruitmentTask.Application.Books.Commands;

public record AddBookCommand(
    string Title,
    decimal Price,
    int BookStand,
    int Shelf,
    IReadOnlyCollection<BookAuthor> Authors): IRequest<ErrorOr<int>>;