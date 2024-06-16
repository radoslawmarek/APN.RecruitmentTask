using APN.RecruitmentTask.Application.Orders.Queries;
using APN.RecruitmentTask.Application.Services;
using APN.RecruitmentTask.Domain.Order;
using FluentAssertions;
using Moq;

namespace APN.RecruitmentTask.Tests.Application;

public class GetOrdersQueryHandlerTests
{
    private readonly Mock<IOrdersRepository> _ordersRepositoryMock = new();
    
    [Fact(DisplayName = "Should return all orders when top is not specified")]
    public async Task ShouldReturnAllOrdersWhenTopIsNotSpecified()
    {
        // Arrange
        GetOrdersQuery query = new();
        var handler = CreateHandler();
        var fakeOrders = CreateFakeOrders().ToList();
        
        _ordersRepositoryMock.Setup(x => x.GetOrders(CancellationToken.None))
            .ReturnsAsync(fakeOrders);
        
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        
        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Item1.Should().BeEquivalentTo(fakeOrders);
        _ordersRepositoryMock.Verify(v => v.GetOrders(CancellationToken.None), Times.Once());
    }
    
    [Fact(DisplayName = "Should return orders by page when top is specified")]
    public async Task ShouldReturnOrdersByPageWhenTopIsSpecified()
    {
        // Arrange
        GetOrdersQuery query = new() { Top = 1, ContinuationToken = "token" };
        var handler = CreateHandler();
        var fakeOrders = CreateFakeOrders().ToList();
        
        _ordersRepositoryMock.Setup(x => x.GetOrdersByPage(1, "token", CancellationToken.None))
            .ReturnsAsync((fakeOrders, "newToken"));
        
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        
        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Item1.Should().BeEquivalentTo(fakeOrders);
        result.Value.Item2.Should().Be("newToken");
        _ordersRepositoryMock.Verify(v => v.GetOrdersByPage(1, "token", CancellationToken.None), Times.Once());
    }
    
    private GetOrdersQueryHandler CreateHandler()
    {
        return new GetOrdersQueryHandler(_ordersRepositoryMock.Object);
    }
    
    private static Order[] CreateFakeOrders()
    {
        return new[]
        {
            new Order
            {
                Id = Guid.NewGuid(),
                OrderLines = new[]
                {
                    new OrderLine { BookId = 1, Quantity = 1 },
                    new OrderLine { BookId = 2, Quantity = 2 }
                }
            },
            new Order
            {
                Id = Guid.NewGuid(),
                OrderLines = new[]
                {
                    new OrderLine { BookId = 3, Quantity = 1 },
                    new OrderLine { BookId = 4, Quantity = 2 }
                }
            }
        };
    }
}