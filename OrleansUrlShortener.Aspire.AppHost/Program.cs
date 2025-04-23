using OrleansUrlShortener.Common;

var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis(Names.Services.Redis);
var orleans = builder.AddOrleans("default")
    .WithClustering(redis)
    .WithGrainStorage(Names.Grains.Storage.Urls, redis)
    .WithGrainStorage(redis);

var server =
    builder.AddProject<Projects.OrleansUrlShortener_Server>("backend")
        .WithReference(orleans)
        .WithReplicas(5);

builder.AddProject<Projects.OrleansUrlShortener_Api>("api")
    .WithExternalHttpEndpoints()
    .WithReference(orleans.AsClient())
    .WaitFor(server);

await builder.Build().RunAsync();