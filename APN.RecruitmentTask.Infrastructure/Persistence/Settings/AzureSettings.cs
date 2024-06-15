namespace APN.RecruitmentTask.Infrastructure.Persistence.Settings;

public class AzureSettings
{
    public string StorageAccountConnectionString { get; set; } = null!;
    public string BooksTableName { get; set; } = null!;
    public string OrdersTableName { get; set; } = null!;
    public IdGenerationSection IdGeneration { get; set; } = null!;
}