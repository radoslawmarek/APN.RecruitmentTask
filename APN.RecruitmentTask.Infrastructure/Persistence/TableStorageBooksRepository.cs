using System.Text.Json;
using APN.RecruitmentTask.Application.Services;
using APN.RecruitmentTask.Domain.Book;
using APN.RecruitmentTask.Infrastructure.Persistence.Model;
using APN.RecruitmentTask.Infrastructure.Persistence.Settings;
using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ErrorOr;

namespace APN.RecruitmentTask.Infrastructure.Persistence;

public class TableStorageBooksRepository(TableServiceClient tableServiceClient, IOptions<AzureSettings> settings, ILogger<TableStorageBooksRepository> logger): IBooksRepository
{
    private readonly AzureSettings _settings = settings.Value;
    private const string PartitionKey = "Books";
    
    public async Task AddBook(Book book, CancellationToken cancellationToken)
    {
        await tableServiceClient.CreateTableIfNotExistsAsync(_settings.BooksTableName, cancellationToken);
        var tableClient = tableServiceClient.GetTableClient(_settings.BooksTableName);
        
        BookEntity bookEntity = new()
        {
            PartitionKey = PartitionKey,
            RowKey = book.Id.ToString(),
            BookSerializedToJson = JsonSerializer.Serialize(book)
        };
        
        await tableClient.AddEntityAsync(bookEntity, cancellationToken: cancellationToken);
    }

    public async Task<ErrorOr<Book>> GetBook(int id, CancellationToken cancellationToken)
    {
        await tableServiceClient.CreateTableIfNotExistsAsync(_settings.BooksTableName, cancellationToken);
        var tableClient = tableServiceClient.GetTableClient(_settings.BooksTableName);
        
        var bookEntity = await tableClient.GetEntityIfExistsAsync<BookEntity>(PartitionKey, id.ToString(), cancellationToken: cancellationToken);
        if (bookEntity is not { HasValue: true })
        {
            return Error.NotFound(code: "Books.NotFound", $"Book with id {id} not found");
        }

        var book = JsonSerializer.Deserialize<Book>(bookEntity.Value!.BookSerializedToJson);
        
        if (book is null)
        {
            throw new ApplicationException($"Cannot deserialize book with id {id} from JSON.");
        }

        return book;
    }

    public async Task<IEnumerable<Book>> GetBooks(CancellationToken cancellationToken)
    {
        await tableServiceClient.CreateTableIfNotExistsAsync(_settings.BooksTableName, cancellationToken);
        var tableClient = tableServiceClient.GetTableClient(_settings.BooksTableName);
        
        var books = new List<Book>();
        
        await foreach (var bookEntity in tableClient.QueryAsync<BookEntity>(filter: $"PartitionKey eq '{PartitionKey}'", cancellationToken: cancellationToken))
        {
            var book = JsonSerializer.Deserialize<Book>(bookEntity.BookSerializedToJson);
            
            if (book is not null)
            {
                books.Add(book);
            }
        }

        return books;
    }
}