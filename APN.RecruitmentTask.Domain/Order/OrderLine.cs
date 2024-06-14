namespace APN.RecruitmentTask.Domain.Order;

public class OrderLine
{
    public required Book.Book Book { get; set; }
    public required int Quantity { get; set; }
}