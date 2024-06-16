namespace APN.RecruitmentTask.Contracts.ApiContracts.Orders;

public record OrderLineRequest
{
    public int BookId { get; set; }
    public int Quantity { get; set; }
};