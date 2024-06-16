using APN.RecruitmentTask.Application.Orders.Commands;
using APN.RecruitmentTask.Application.Orders.Commands.Validators;
using APN.RecruitmentTask.Application.Services;
using APN.RecruitmentTask.Domain.Book;
using APN.RecruitmentTask.Domain.Order;
using FluentAssertions;
using Moq;

namespace APN.RecruitmentTask.Tests.Application;

public class AddOrderCommandHandlerTests
{
    private readonly Mock<IOrdersRepository> _ordersRepositoryMock = new();
    private readonly Mock<IBooksRepository> _booksRepositoryMock = new();
    private readonly AddOrderCommandValidator _commandValidator = new();
    
    [Fact(DisplayName = "Should return validation error when command is invalid")]
    public async Task ShouldReturnValidationErrorWhenCommandIsInvalid()
    {
        // Arrange
        AddOrderCommand command = new()
        {
            OrderLines = []
        };
        var handler = CreateHandler();
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == $"{nameof(AddOrderCommandHandler)}.ValidationFailed");
    }
    
    [Fact(DisplayName = "Should return error when some books are not found")]
    public async Task ShouldReturnErrorWhenSomeBooksAreNotFound()
    {
        // Arrange
        AddOrderCommand command = CreateFakeCommand();
        var handler = CreateHandler();
        var fakeBooks = CreateFakeBooks().ToList();
        fakeBooks.Add(new Book
        {
            Id = 999, BookStand = 1, Shelf = 1, Price = 10.2M, Title = "ddd",
            Authors = [new BookAuthor { FirstName = "Author", LastName = "Fourth" }]
        });
        
        _booksRepositoryMock.Setup(x => x.GetBooksByIds(new[] { 1 }, CancellationToken.None))
            .ReturnsAsync(fakeBooks.ToArray());
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == $"{nameof(AddOrderCommandHandler)}.BooksNotFound");
    }
    
    [Fact(DisplayName = "Should add order to repository and return its id")]
    public async Task ShouldAddOrderToRepositoryAndReturnItsId()
    {
        // Arrange
        AddOrderCommand command = CreateFakeCommand();
        var handler = CreateHandler();
        
        _booksRepositoryMock.Setup(x => x.GetBooksByIds(new[] { 1001, 1002, 1003 }, CancellationToken.None))
            .ReturnsAsync(CreateFakeBooks());
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();
    }
    
    private AddOrderCommandHandler CreateHandler()
    {
        return new AddOrderCommandHandler(_ordersRepositoryMock.Object, _booksRepositoryMock.Object, _commandValidator);
    }
    
    private static AddOrderCommand CreateFakeCommand()
    {
        return new AddOrderCommand
        {
            OrderLines = [
                new OrderLine { BookId = 1001, Quantity = 1 },
                new OrderLine { BookId = 1002, Quantity = 2 },
                new OrderLine { BookId = 1003, Quantity = 3 }
            ]
        };
    }
    
    private static Book[] CreateFakeBooks()
    {
        return new[]
        {
            new Book { Id = 1001, BookStand = 1, Shelf = 1, Price = 10.2M, Title = "aaaa", Authors = [ new BookAuthor { FirstName = "Author", LastName = "One"} ]},
            new Book { Id = 1002, BookStand = 1, Shelf = 1, Price = 10.2M, Title = "bbb", Authors = [ new BookAuthor { FirstName = "Author", LastName = "Second"} ]},
            new Book { Id = 1003, BookStand = 1, Shelf = 1, Price = 10.2M, Title = "ccc", Authors = [ new BookAuthor { FirstName = "Author", LastName = "Third"} ]}
        };
    }
}