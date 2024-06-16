using APN.RecruitmentTask.Application.Services;
using APN.RecruitmentTask.Domain.Order;
using ErrorOr;
using MediatR;

namespace APN.RecruitmentTask.Application.Orders.Queries;

public class GetOrdersQueryHandler(IOrdersRepository ordersRepository): IRequestHandler<GetOrdersQuery, ErrorOr<IEnumerable<Order>>>
{
    public async Task<ErrorOr<IEnumerable<Order>>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        return (await ordersRepository.GetOrders(cancellationToken))
            .ToArray();
    }
}