using APN.RecruitmentTask.Application.Services;
using APN.RecruitmentTask.Domain.Book;
using MediatR;
using ErrorOr;

namespace APN.RecruitmentTask.Application.Books.Commands;

public class AddBookCommandHandler(IBooksRepository booksRepository, IUniqueIdGenerator<int> idGenerator): IRequestHandler<AddBookCommand, ErrorOr<int>>
{
    public async Task<ErrorOr<int>> Handle(AddBookCommand request, CancellationToken cancellationToken)
    {
        var id = await idGenerator.GenerateUniqueId("book", cancellationToken);
        var existingBookResult = await booksRepository.GetBook(id, cancellationToken);

        if (!existingBookResult.IsError)
        {
            throw new ApplicationException($"Trying to add book with id {id} that already exists.");
        }
        
        await booksRepository.AddBook(new Book
        {
            Id = id,
            Price = request.Price,
            BookStand = request.BookStand,
            Shelf = request.Shelf,
            Title = request.Title,
            Authors = request.Authors
        }, cancellationToken);
        
        return await Task.FromResult(id);
    }
}