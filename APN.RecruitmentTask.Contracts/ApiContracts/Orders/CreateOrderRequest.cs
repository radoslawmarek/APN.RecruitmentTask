namespace APN.RecruitmentTask.Contracts.ApiContracts.Orders;

public record CreateOrderRequest(IEnumerable<OrderLineRequest> OrderLines);