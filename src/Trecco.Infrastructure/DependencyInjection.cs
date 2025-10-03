using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Trecco.Application;
using Trecco.Application.Common.Abstractions;
using Trecco.Application.Common.Endpoints;
using Trecco.Domain.BoardActionLogs;
using Trecco.Domain.Boards;
using Trecco.Infrastructure.Infrastructure;
using Trecco.Infrastructure.Infrastructure.Extensions;
using Trecco.Infrastructure.Infrastructure.Hubs;
using Trecco.Infrastructure.Infrastructure.Repositories;

namespace Trecco.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string dbConnectionString = configuration.GetConnectionStringOrThrow(ServiceNames.MongoDb);

        services.AddSingleton<IMongoClient>(sp => new MongoClient(dbConnectionString));
        services.AddSingleton(sp =>
        {
            IMongoClient client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(ServiceNames.DatabaseName);
        });

        services.AddHttpContextAccessor();

        services.AddSignalR();
        services.AddSingleton<IBoardNotifier, BoardHubNotifier>();

        services.AddSingleton<IBoardRepository, BoardRepository>();
        services.AddSingleton<IBoardActionLogRepository, BoardActionLogRepository>();

        services.AddEndpoints(AssemblyReference.Assembly);

        return services;
    }
}
