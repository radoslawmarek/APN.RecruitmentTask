using System.Net.Http.Json;
using APN.RecruitmentTask.Application.Orders.Queries;
using APN.RecruitmentTask.Contracts.ApiContracts.Orders;
using APN.RecruitmentTask.Domain.Order;
using ErrorOr;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace APN.RecruitmentTask.Tests.Api;

public class OrdersEndpointTests(CustomWebApplicationFactory<Program> factory): IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory = factory;
    private readonly Mock<IMediator> _mediatorMock = new();
    
    [Fact(DisplayName = "Should return all orders when top is not specified")]
    public async Task ShouldReturnAllOrdersWhenTopIsNotSpecified()
    {
        string? continuationToken = null;
        
        _mediatorMock.Setup(s => s.Send(It.IsAny<GetOrdersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ErrorOrFactory.From((CreateFakeOrders(), continuationToken)));
        
        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(_mediatorMock.Object);
            });
        }).CreateClient();
        
        // Act
        var response = await client.GetAsync("/api/orders");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var orders = await response.Content.ReadFromJsonAsync<IEnumerable<OrderQueryResult>>();
        orders.Should().BeEquivalentTo(CreateFakeOrdersQueryResult());
    }
    
    [Fact(DisplayName = "Should return orders by page when top is specified")]
    public async Task ShouldReturnOrdersByPageWhenTopIsSpecified()
    {
        string? continuationToken = null;
        
        _mediatorMock.Setup(s => s.Send(It.IsAny<GetOrdersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ErrorOrFactory.From((CreateFakeOrders(), continuationToken ?? "token")));
        
        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(_mediatorMock.Object);
            });
        }).CreateClient();
        
        // Act
        var response = await client.GetAsync("/api/orders?top=1");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var orders = await response.Content.ReadFromJsonAsync<IEnumerable<OrderQueryResult>>();
        orders.Should().BeEquivalentTo(CreateFakeOrdersQueryResult());
        response.Headers.GetValues("X-Continuation-Token").Should().Contain("token");
    }
    
    private static IEnumerable<OrderQueryResult> CreateFakeOrdersQueryResult()
    {
        return new[]
        {
            new OrderQueryResult(Guid.Parse("ABF3D94B-8261-4E24-B4A0-38FDC310B7D0"), new[] { new OrderLineQueryResult(1, 1) }),
            new OrderQueryResult(Guid.Parse("A06FC055-8580-455D-B400-83CC878D92C9"), new[] { new OrderLineQueryResult(2, 2) })
        };
    }
    
    private static IEnumerable<Order> CreateFakeOrders()
    {
        return new[]
        {
            new Order { Id = Guid.Parse("ABF3D94B-8261-4E24-B4A0-38FDC310B7D0"), OrderLines = [new OrderLine { BookId = 1, Quantity = 1} ] },
            new Order { Id = Guid.Parse("A06FC055-8580-455D-B400-83CC878D92C9"), OrderLines = [new OrderLine { BookId = 2, Quantity = 2} ] }
        };
    }
}