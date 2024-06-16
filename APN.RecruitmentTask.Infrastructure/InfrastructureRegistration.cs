using APN.RecruitmentTask.Application.Services;
using APN.RecruitmentTask.Infrastructure.Persistence;
using APN.RecruitmentTask.Infrastructure.Persistence.Settings;
using Azure.Identity;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace APN.RecruitmentTask.Infrastructure;

public static class InfrastructureRegistration
{
    public static void RegisterInfrastructure(this IServiceCollection services, IConfiguration configuration, string sectionName = "Azure")
    {
        AzureSettings azureSettings = new();
        configuration.GetSection(sectionName).Bind(azureSettings);
        services.AddSingleton(Options.Create(azureSettings));
        
        services.AddTransient<IBooksRepository, TableStorageBooksRepository>();
        services.AddTransient<IOrdersRepository, TableStorageOrdersRepository>();
        services.AddTransient<IUniqueIdGenerator<int>, TableStorageUniqueIdGenerator>();
        
        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddTableServiceClient("UseDevelopmentStorage=true");
            clientBuilder.UseCredential(new DefaultAzureCredential());
        });
    }
}