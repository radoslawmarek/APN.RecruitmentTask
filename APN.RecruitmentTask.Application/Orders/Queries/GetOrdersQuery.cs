using APN.RecruitmentTask.Domain.Order;
using MediatR;
using ErrorOr;

namespace APN.RecruitmentTask.Application.Orders.Queries;

public record GetOrdersQuery(): IRequest<ErrorOr<IEnumerable<Order>>>;