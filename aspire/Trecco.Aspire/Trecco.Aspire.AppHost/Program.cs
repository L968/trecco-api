using Trecco.Application.Infrastructure;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<ParameterResource> mongoPassword = builder.AddParameter("mongoPassword", "root", secret: true);
IResourceBuilder<ParameterResource> redisPassword = builder.AddParameter("redisPassword", "root", secret: true);
IResourceBuilder<ParameterResource> keycloakPassword = builder.AddParameter("keycloakPassword", "root", secret: true);

IResourceBuilder<MongoDBServerResource> mongo = builder.AddMongoDB(ServiceNames.Mongo, password: mongoPassword)
    .WithImageTag("8.0.13")
    .WithMongoExpress()
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

IResourceBuilder<MongoDBDatabaseResource> treccodb = mongo.AddDatabase(ServiceNames.MongoDb, ServiceNames.DatabaseName);

IResourceBuilder<KeycloakResource> keycloak = builder.AddKeycloak(ServiceNames.Keycloak, port: 8080, adminPassword: keycloakPassword)
    .WithImageTag("26.3")
    .WithDataVolume()
    .WithExternalHttpEndpoints()
    .WithLifetime(ContainerLifetime.Persistent);

IResourceBuilder<RedisResource> redis = builder.AddRedis(ServiceNames.Redis, password: redisPassword)
    .WithImageTag("8.2.1")
    .WithRedisInsight(insight => insight.WithHostPort(5540))
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

builder.AddProject<Projects.Trecco_Api>(ServiceNames.Api)
    .WithReference(treccodb)
        .WaitFor(treccodb)
    .WithReference(keycloak)
        .WaitFor(keycloak)
    .WithReference(redis)
        .WaitFor(redis);

await builder.Build().RunAsync();
