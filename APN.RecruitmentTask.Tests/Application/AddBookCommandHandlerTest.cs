using APN.RecruitmentTask.Application.Books.Commands;
using APN.RecruitmentTask.Application.Services;
using APN.RecruitmentTask.Domain.Book;
using FluentAssertions;
using Moq;
using ErrorOr;

namespace APN.RecruitmentTask.Tests.Application;

public class AddBookCommandHandlerTest
{
    private readonly Mock<IBooksRepository> _booksRepositoryMock = new();
    private readonly Mock<IUniqueIdGenerator<int>> _uniqueIdGeneratorMock = new();
    private readonly AddBookCommandValidator _commandValidator = new();
    
    [Fact(DisplayName = "Should return validation error when command is invalid")] 
    public async Task ShouldReturnValidationErrorWhenCommandIsInvalid()
    {
        // Arrange
        var command = new AddBookCommand(string.Empty, 0, 0, 0, new List<BookAuthor>());
        var handler = CreateHandler();
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == $"{nameof(AddBookCommandHandler)}.ValidationFailed");
    }
    
    [Fact(DisplayName = "Should return error when book with generated id already exists")]
    public async Task ShouldReturnErrorWhenBookWithGeneratedIdAlreadyExists()
    {
        // Arrange
        var command = CreateFakeCommand();
        var handler = CreateHandler();
        
        _booksRepositoryMock.Setup(x => x.GetBook(1001, CancellationToken.None))
            .ReturnsAsync(CreateFakeBook());
        
        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ApplicationException>()
            .WithMessage("Trying to add book with id 1001 that already exists.");
    }
    
    [Fact(DisplayName = "Should add book to repository and return its id")]
    public async Task ShouldAddBookToRepositoryAndReturnItsId()
    {
        // Arrange
        var command = CreateFakeCommand();
        var handler = CreateHandler();
        
        _booksRepositoryMock.Setup(x => x.GetBook(1001, CancellationToken.None))
            .ReturnsAsync(Error.NotFound("Books.NotFound", "Book with id 1001 not found"));
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(1001);
        
        _booksRepositoryMock.Verify(x => x.AddBook(It.Is<Book>(b => b.Id == 1001), CancellationToken.None), Times.Once);
    }
    
    private AddBookCommandHandler CreateHandler()
    {
        _uniqueIdGeneratorMock.Setup(x => x.GenerateUniqueId("book", CancellationToken.None))
            .ReturnsAsync(1001);
        
        return new AddBookCommandHandler(_booksRepositoryMock.Object, _uniqueIdGeneratorMock.Object, _commandValidator);
    }

    private static AddBookCommand CreateFakeCommand()
    {
        return new AddBookCommand("Title", 1, 1, 1, [
            new BookAuthor { FirstName = "Author", LastName = "One" }
        ]);
    }

    private static Book CreateFakeBook()
    {
        return new Book
        {
            Id = 1001,
            Title = "Title One",
            Price = 10.34M,
            Shelf = 2,
            BookStand = 3,
            Authors = [
                new BookAuthor { FirstName = "Author", LastName = "One" }
            ]
        };
    }
}