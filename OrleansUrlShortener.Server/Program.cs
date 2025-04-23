using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using OrleansUrlShortener.Common;

var builder = Host.CreateApplicationBuilder(args);

builder.AddKeyedRedisClient(Names.Services.Redis);
builder.AddServiceDefaults();

builder.UseOrleans(static siloBuilder =>
{
    siloBuilder.AddActivityPropagation();
});

builder.Services.AddSingleton<IHostLifetime, ConsoleLifetime>();

await builder.Build().RunAsync();