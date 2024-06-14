namespace APN.RecruitmentTask.Domain;

public abstract class EntityBase<TId>
{
    public required TId Id { get; set; }
}