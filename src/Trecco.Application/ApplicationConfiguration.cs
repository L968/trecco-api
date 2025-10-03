using System.Globalization;
using System.Reflection;
using MediatR.NotificationPublishers;
using Microsoft.Extensions.DependencyInjection;
using Trecco.Application.Common.Authentication;
using Trecco.Application.Common.Behaviors;
using Trecco.Application.Common.DomainEvents;

namespace Trecco.Application;

public static class ApplicationConfiguration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        Assembly assembly = AssemblyReference.Assembly;

        services.AddMediatR(config =>
        {
            config.NotificationPublisher = new TaskWhenAllPublisher();
            config.NotificationPublisherType = typeof(TaskWhenAllPublisher);

            config.RegisterServicesFromAssembly(assembly);

            config.AddOpenBehavior(typeof(PerformanceBehavior<,>));
            config.AddOpenBehavior(typeof(UserContextBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
        });


        services.AddSingleton<IUserContext, UserContext>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);
        ValidatorOptions.Global.LanguageManager.Culture = CultureInfo.InvariantCulture;

        return services;
    }
}
