namespace APN.RecruitmentTask.Contracts.ApiContracts.Orders;

public record OrderQueryResult(Guid Id, 
    IEnumerable<OrderLineQueryResult> OrderLines
    );