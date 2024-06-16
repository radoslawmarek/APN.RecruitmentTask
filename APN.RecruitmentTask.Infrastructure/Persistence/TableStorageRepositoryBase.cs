using APN.RecruitmentTask.Infrastructure.Persistence.Settings;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;

namespace APN.RecruitmentTask.Infrastructure.Persistence;

public abstract class TableStorageRepositoryBase(TableServiceClient tableServiceClient, IOptions<AzureSettings> azureSettingsOptions)
{
    protected readonly AzureSettings AzureSettings = azureSettingsOptions.Value;
    
    protected virtual async Task<TableClient> CreateTableClient(string tableName, CancellationToken cancellationToken)
    {
        await tableServiceClient.CreateTableIfNotExistsAsync(tableName, cancellationToken);
        return tableServiceClient.GetTableClient(tableName);
    }
}