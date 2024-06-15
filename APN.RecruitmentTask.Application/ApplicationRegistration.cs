using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace APN.RecruitmentTask.Application;

public static class ApplicationRegistration
{
    public static void RegisterApplication(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(typeof(ApplicationRegistration).Assembly);
        });

        services.AddValidatorsFromAssembly(typeof(ApplicationRegistration).Assembly);
    }
}