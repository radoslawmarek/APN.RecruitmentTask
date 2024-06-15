using APN.RecruitmentTask.Domain.Order;
using ErrorOr;
using MediatR;

namespace APN.RecruitmentTask.Application.Orders.Queries;

public class GetOrdersQueryHandler: IRequestHandler<GetOrdersQuery, ErrorOr<IEnumerable<Order>>>
{
    public async Task<ErrorOr<IEnumerable<Order>>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(new List<Order>());
    }
}