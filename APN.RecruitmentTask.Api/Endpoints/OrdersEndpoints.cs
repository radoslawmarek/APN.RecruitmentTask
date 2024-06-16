using APN.RecruitmentTask.Application.Orders.Commands;
using APN.RecruitmentTask.Application.Orders.Queries;
using APN.RecruitmentTask.Contracts.ApiContracts.Orders;
using APN.RecruitmentTask.Domain.Order;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APN.RecruitmentTask.Api.Endpoints;

public static class OrdersEndpoints
{
    private static readonly string[] EndpointTags = new[] { "Orders" };
    
    public static void AddOrdersEndpoints(this WebApplication application)
    {
        application.MapGet("/api/orders", [Authorize] async ([FromServices] IMediator mediator, [FromQuery] int? top, HttpRequest request, HttpResponse response) =>
            {
                var continuationToken = request.Headers["X-Continuation-Token"].FirstOrDefault();
                
                var result = await mediator.Send(new GetOrdersQuery(top, continuationToken));
                return result.Match(
                    successResponse =>
                    {
                        var (orders, continuationToken) = successResponse;
                        if (continuationToken != null)
                        {
                            response.Headers.Append("X-Continuation-Token", continuationToken);
                        }
                        
                        return Results.Ok(orders.Select(o => new OrderQueryResult(o.Id,
                            o.OrderLines.Select(ol => new OrderLineQueryResult(ol.BookId, ol.Quantity))
                        )));
                    },
                    errors => Results.BadRequest(string.Join(", ", errors.Select(e => e.Description)))
                );
            })
            .WithName("GetOrders")
            .WithDescription("Get list of orders")
            .WithTags(EndpointTags)
            .Produces<IEnumerable<OrderQueryResult>>();
        
        application.MapPost("/api/orders", [Authorize] async ([FromServices] IMediator mediator, CreateOrderRequest request) =>
            {
                var command = new AddOrderCommand
                {
                    OrderLines = request.OrderLines.Select(x => new OrderLine
                        {
                            BookId = x.BookId,
                            Quantity = x.Quantity
                        }
                   )
                };
                
                var result = await mediator.Send(command);
                return result.Match(
                    orderId => Results.Created($"/api/orders/{orderId}", orderId),
                    errors => Results.BadRequest(string.Join(", ", errors.Select(e => e.Description))
                    )
                );
            })
            .WithName("AddOrder")
            .WithDescription("Add new order")
            .WithTags(EndpointTags)
            .Produces<Guid>();
    }
}