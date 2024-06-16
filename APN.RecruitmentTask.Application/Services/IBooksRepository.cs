using APN.RecruitmentTask.Domain.Book;
using ErrorOr;

namespace APN.RecruitmentTask.Application.Services;

public interface IBooksRepository
{
    public Task AddBook(Book book, CancellationToken cancellationToken);
    public Task<ErrorOr<Book>> GetBook(int id, CancellationToken cancellationToken);
    public Task<IEnumerable<Book>> GetBooks(CancellationToken cancellationToken);
    public Task<IEnumerable<Book>> GetBooksByIds(IEnumerable<int> ids, CancellationToken cancellationToken);
}