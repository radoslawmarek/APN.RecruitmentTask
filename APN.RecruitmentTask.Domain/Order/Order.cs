namespace APN.RecruitmentTask.Domain.Order;

public class Order: EntityBase<Guid>
{
    public required IEnumerable<OrderLine> OrderLines { get; set; }
}