using APN.RecruitmentTask.Application.Orders.Queries;
using APN.RecruitmentTask.Contracts.ApiContracts.Orders;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace APN.RecruitmentTask.Api.Endpoints;

public static class OrdersEndpoints
{
    public static void AddOrdersEndpoints(this WebApplication application)
    {
        application.MapGet("/api/orders", async ([FromServices] IMediator mediator) =>
            {
                var result = await mediator.Send(new GetOrdersQuery());
                result.Match(
                    orderList => Results.Ok(orderList),
                    errors => Results.BadRequest(string.Join(", ", errors.Select(e => e.Description)))
                );
            })
            .WithName("GetOrders")
            .WithDescription("Get list of orders")
            .WithTags(new[] { "Orders" })
            .Produces<IEnumerable<OrderQueryResult>>();
    }
}