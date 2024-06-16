// See https://aka.ms/new-console-template for more information

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using APN.RecruitmentTask.Contracts.ApiContracts.Books;
using APN.RecruitmentTask.Contracts.ApiContracts.Orders;
using APN.RecruitmentTask.OrdersGenerator;
using Bogus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var settings = GetAppSettings();
var httpClientFactory = GetHttpClientFactory();

var books = await GetBooksAsync(httpClientFactory, settings.ApiUrl, settings.ApiToken);
var booksIds = books?.Select(x => x.Id).ToList();

var orderLineFaker = new Faker<OrderLineRequest>()
    .RuleFor(f => f.Quantity, x => x.Random.Number(1, 123))
    .RuleFor(f => f.BookId, x => x.PickRandom(booksIds));

var orderFaker = new Faker<CreateOrderRequest>()
    .RuleFor(f => f.OrderLines, x => orderLineFaker.Generate(x.Random.Number(1, 100)));

foreach (var order in orderFaker.Generate(1000))
{
    Console.WriteLine(await AddOrderAsync(httpClientFactory, settings.ApiUrl, settings.ApiToken, order));
}

return;

static async Task<IEnumerable<BookQueryResult>?> GetBooksAsync(IHttpClientFactory httpClientFactory, string apiUrl, string bearerToken)
{
    var client = httpClientFactory.CreateClient();

    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

    var response = await client.GetAsync($"{apiUrl}/api/books");
    response.EnsureSuccessStatusCode();

    return await response.Content.ReadFromJsonAsync<IEnumerable<BookQueryResult>>(CreateJsonSerializerOptions());
}

static async Task<string> AddOrderAsync(IHttpClientFactory httpClientFactory, string apiUrl, string bearerToken, CreateOrderRequest newOrder)
{
    var client = httpClientFactory.CreateClient();

    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

    var response = await client.PostAsJsonAsync($"{apiUrl}/api/orders", newOrder, CreateJsonSerializerOptions());
    response.EnsureSuccessStatusCode();

    return await response.Content.ReadAsStringAsync();
}




static JsonSerializerOptions CreateJsonSerializerOptions()
{
    var options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    return options;
}

IHttpClientFactory GetHttpClientFactory()
{
    var services = new ServiceCollection()
        .AddHttpClient()
        .BuildServiceProvider();

    return services.GetRequiredService<IHttpClientFactory>();
}

AppSettings GetAppSettings()
{
    var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";

    var configuration = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables()
        .Build();

    var appSettings = new AppSettings();
    configuration.GetSection("AppSettings").Bind(appSettings);

    return appSettings;
}