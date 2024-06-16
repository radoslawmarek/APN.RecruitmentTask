namespace APN.RecruitmentTask.Domain.Order;

public class OrderLine
{
    public required int BookId { get; set; }
    public required int Quantity { get; set; }
}