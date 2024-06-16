using APN.RecruitmentTask.Domain.Order;
using MediatR;
using ErrorOr;

namespace APN.RecruitmentTask.Application.Orders.Commands;

public record AddOrderCommand: IRequest<ErrorOr<Guid>>
{
    public required IEnumerable<OrderLine> OrderLines { get; init; }
} 