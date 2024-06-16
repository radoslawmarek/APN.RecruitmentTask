using APN.RecruitmentTask.Domain.Order;
using MediatR;
using ErrorOr;

namespace APN.RecruitmentTask.Application.Orders.Queries;

public record GetOrdersQuery(int? Top = null, string? ContinuationToken = null): IRequest<ErrorOr<(IEnumerable<Order>, string?)>>;