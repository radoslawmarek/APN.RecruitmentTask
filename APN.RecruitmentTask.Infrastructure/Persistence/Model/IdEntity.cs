using Azure;
using Azure.Data.Tables;

namespace APN.RecruitmentTask.Infrastructure.Persistence.Model;

public class IdEntity: ITableEntity
{
    public string? PartitionKey { get; set; }
    public string? RowKey { get; set; }
    public int IdCounter { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}