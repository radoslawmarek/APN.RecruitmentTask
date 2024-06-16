using APN.RecruitmentTask.Application.Services;
using APN.RecruitmentTask.Domain.Order;
using ErrorOr;
using MediatR;

namespace APN.RecruitmentTask.Application.Orders.Queries;

public class GetOrdersQueryHandler(IOrdersRepository ordersRepository): IRequestHandler<GetOrdersQuery, ErrorOr<(IEnumerable<Order>, string?)>>
{
    public async Task<ErrorOr<(IEnumerable<Order>, string?)>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        if (!request.Top.HasValue)
            return ((await ordersRepository.GetOrders(cancellationToken)).ToArray(), null);
        
        var (orders, continuationToken) = await ordersRepository.GetOrdersByPage(request.Top.Value, request.ContinuationToken, cancellationToken);
        return (orders.ToArray(), continuationToken);
    }
}