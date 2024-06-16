namespace APN.RecruitmentTask.Contracts.ApiContracts.Orders;

public record CreateOrderRequest
{
    public required IEnumerable<OrderLineRequest> OrderLines { get; init; }
};