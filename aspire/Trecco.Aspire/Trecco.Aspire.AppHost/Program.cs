using Trecco.Application.Infrastructure;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<ParameterResource> mongoPassword = builder.AddParameter("mongoPassword", "root", secret: true);

IResourceBuilder<MongoDBServerResource> mongo = builder.AddMongoDB(ServiceNames.Mongo, port: 27017, password: mongoPassword)
    .WithImageTag("8.0.13")
    .WithDataVolume()
    .WithExternalHttpEndpoints()
    .WithLifetime(ContainerLifetime.Persistent);

IResourceBuilder<MongoDBDatabaseResource> treccodb = mongo.AddDatabase(ServiceNames.MongoDb, ServiceNames.DatabaseName);

builder.AddProject<Projects.Trecco_Api>(ServiceNames.Api)
    .WithReference(treccodb)
        .WaitFor(treccodb);

await builder.Build().RunAsync();
