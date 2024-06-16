using APN.RecruitmentTask.Domain.Order;
using ErrorOr;

namespace APN.RecruitmentTask.Application.Services;

public interface IOrdersRepository
{
    Task<IEnumerable<Order>> GetOrders(CancellationToken cancellationToken);
    Task<ErrorOr<Order>> GetOrder(Guid id, CancellationToken cancellationToken);
    Task AddOrder(Order order, CancellationToken cancellationToken);
}