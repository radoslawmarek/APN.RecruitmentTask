using System.Collections;
using APN.RecruitmentTask.Domain.Order;
using ErrorOr;

namespace APN.RecruitmentTask.Application.Services;

public interface IOrdersRepository
{
    Task<IEnumerable<Order>> GetOrders(CancellationToken cancellationToken);
    Task<(IEnumerable<Order> orders, string? continuationToken)> GetOrdersByPage(int top, string? continuationToken, CancellationToken cancellationToken);
    Task<ErrorOr<Order>> GetOrder(Guid id, CancellationToken cancellationToken);
    Task AddOrder(Order order, CancellationToken cancellationToken);
}