using APN.RecruitmentTask.Application.Services;
using APN.RecruitmentTask.Infrastructure.Persistence.Model;
using APN.RecruitmentTask.Infrastructure.Persistence.Settings;
using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace APN.RecruitmentTask.Infrastructure.Persistence;

public class TableStorageUniqueIdGenerator(TableServiceClient tableServiceClient, IOptions<AzureSettings> settings, ILogger<TableStorageUniqueIdGenerator> logger): IUniqueIdGenerator<int>
{
    private readonly AzureSettings _settings = settings.Value;
    private const string PartitionKey = "IdGeneration";
    
    // The implementation below is naive and I would rather not use it in a production system. 
    // I leave it in this state due to lack of time. Normally, I would use e.g. Redis or SQL (with sequences).
    public async Task<int> GenerateUniqueId(string idName, CancellationToken cancellationToken = default)
    {
        await tableServiceClient.CreateTableIfNotExistsAsync(_settings.IdGeneration.TableName, cancellationToken);
        var tableClient = tableServiceClient.GetTableClient(_settings.IdGeneration.TableName);

        while (true)
        {
            try
            {
                var idEntity = await tableClient.GetEntityIfExistsAsync<IdEntity>(PartitionKey, idName, cancellationToken: cancellationToken);
                if (idEntity is not { HasValue: true })
                {
                    IdEntity newIdEntity = new()
                    {
                        PartitionKey = PartitionKey,
                        RowKey = idName,
                        IdCounter = 1
                    };
                    
                    await tableClient.AddEntityAsync(newIdEntity, cancellationToken: cancellationToken);
                    logger.LogInformation("New id for {IdName} generated: {IdCounter}", idName, newIdEntity.IdCounter);

                    return newIdEntity.IdCounter;
                }

                idEntity.Value!.IdCounter++;
                await tableClient.UpdateEntityAsync(idEntity.Value, ETag.All, cancellationToken: cancellationToken);
                logger.LogInformation("Id for {IdName} updated: {IdCounter}", idName, idEntity.Value.IdCounter);
                
                return idEntity.Value.IdCounter;
            } 
            catch(RequestFailedException ex) when (ex.Status == 412)
            {
                logger.LogWarning("Conflict while updating entity. Retrying...");
                await Task.Delay(100, cancellationToken);
            }
            
            throw new ApplicationException($"Cannot generate unique id for: {idName}");
        }
    }
}