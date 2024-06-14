using APN.RecruitmentTask.Domain.Book;
using MediatR;
using ErrorOr;

namespace APN.RecruitmentTask.Application.Books.Queries;

public record GetBooksQuery(): IRequest<ErrorOr<IEnumerable<Book>>>;