using Azure;
using Azure.Data.Tables;

namespace APN.RecruitmentTask.Infrastructure.Persistence.Model;

public record BookEntity : ITableEntity
{
    public required string PartitionKey { get; set; }
    public required string RowKey { get; set; }
    public required string BookSerializedToJson { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}