namespace APN.RecruitmentTask.Application.Services;

public interface IUniqueIdGenerator<T>
{
    Task<T> GenerateUniqueId(string idName, CancellationToken cancellationToken = default);
}