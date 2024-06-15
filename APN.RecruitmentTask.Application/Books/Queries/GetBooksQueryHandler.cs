using APN.RecruitmentTask.Domain.Book;
using MediatR;
using ErrorOr;

namespace APN.RecruitmentTask.Application.Books.Queries;

public class GetBooksQueryHandler: IRequestHandler<GetBooksQuery, ErrorOr<IEnumerable<Book>>>
{
    public async Task<ErrorOr<IEnumerable<Book>>> Handle(GetBooksQuery request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(new List<Book>());
    }
}