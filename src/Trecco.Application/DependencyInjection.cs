using System.Globalization;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Trecco.Application.Common.Behaviors;
using Trecco.Application.Common.Endpoints;
using Trecco.Application.Common.Extensions;
using Trecco.Application.Domain.Boards;
using Trecco.Application.Infrastructure;
using Trecco.Application.Infrastructure.Repositories;

namespace Trecco.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        Assembly assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);

            config.AddOpenBehavior(typeof(PerformanceBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
        });

        services.AddSignalR();

        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);
        ValidatorOptions.Global.LanguageManager.Culture = CultureInfo.InvariantCulture;

        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string dbConnectionString = configuration.GetConnectionStringOrThrow(ServiceNames.MongoDb);

        services.AddSingleton<IMongoClient>(sp => new MongoClient(dbConnectionString));
        services.AddSingleton(sp =>
        {
            IMongoClient client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(ServiceNames.DatabaseName);
        });

        services.AddSingleton<IBoardRepository, BoardRepository>();

        services.AddEndpoints(typeof(DependencyInjection).Assembly);

        return services;
    }
}
