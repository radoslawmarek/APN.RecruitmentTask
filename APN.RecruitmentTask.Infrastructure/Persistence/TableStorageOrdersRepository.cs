using System.Text.Json;
using APN.RecruitmentTask.Application.Services;
using APN.RecruitmentTask.Domain.Order;
using APN.RecruitmentTask.Infrastructure.Persistence.Model;
using APN.RecruitmentTask.Infrastructure.Persistence.Settings;
using Azure.Data.Tables;
using ErrorOr;
using Microsoft.Extensions.Options;

namespace APN.RecruitmentTask.Infrastructure.Persistence;

public class TableStorageOrdersRepository(TableServiceClient tableServiceClient, IOptions<AzureSettings> options):
    TableStorageRepositoryBase(tableServiceClient, options),  IOrdersRepository
{
    private const string PartitionKey = "Orders";
    
    public async Task<IEnumerable<Order>> GetOrders(CancellationToken cancellationToken)
    {
        var tableClient = await CreateTableClient(AzureSettings.OrdersTableName, cancellationToken);
        
        var orders = new List<Order>();
        
        await foreach (var orderEntity in tableClient.QueryAsync<OrderEntity>(filter: $"PartitionKey eq '{PartitionKey}'",cancellationToken: cancellationToken))
        {
            var order = JsonSerializer.Deserialize<Order>(orderEntity.OrderSerializedToJson);
            
            if (order is null)
            {
                throw new ApplicationException($"Cannot deserialize order with id {orderEntity.RowKey} from JSON.");
            }
            
            orders.Add(order);
        }
        
        return orders;
    }

    public async Task<ErrorOr<Order>> GetOrder(Guid id, CancellationToken cancellationToken)
    {
        var tableClient = await CreateTableClient(AzureSettings.OrdersTableName, cancellationToken);
        
        var orderEntity = await tableClient.GetEntityIfExistsAsync<OrderEntity>(PartitionKey, id.ToString(), cancellationToken: cancellationToken);
        if (orderEntity is not { HasValue: true })
        {
            return Error.NotFound(code: "Orders.NotFound", $"Order with id {id} not found");
        }
        
        var order = JsonSerializer.Deserialize<Order>(orderEntity.Value!.OrderSerializedToJson);
        
        if (order is null)
        {
            throw new ApplicationException($"Cannot deserialize order with id {id} from JSON.");
        }
        
        return order;
    }

    public async Task AddOrder(Order order, CancellationToken cancellationToken)
    {

        var tableClient = await CreateTableClient(AzureSettings.OrdersTableName, cancellationToken);
        
        OrderEntity orderEntity = new()
        {
            PartitionKey = PartitionKey,
            RowKey = order.Id.ToString(),
            OrderSerializedToJson = JsonSerializer.Serialize(order)
        };
        
        await tableClient.AddEntityAsync(orderEntity, cancellationToken: cancellationToken);
    }
}