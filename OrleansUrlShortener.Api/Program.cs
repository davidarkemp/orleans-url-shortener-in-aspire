using Microsoft.AspNetCore.Mvc;
using OrleansUrlShortener.Common;
using OrleansUrlShortener.Common.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.AddKeyedRedisClient(Names.Services.Redis);
builder.AddServiceDefaults();
builder.Host.UseOrleansClient(clientBuilder => clientBuilder.AddActivityPropagation());

var app = builder.Build();

app.MapGet("/", static () => "Welcome to the URL shortener, powered by Orleans!");

app.MapGet("/shorten",
    static async ([FromServices] IGrainFactory grains, HttpRequest request, [FromQuery] string url) =>
    {
        var host = $"{request.Scheme}://{request.Host.Value}";

        // Validate the URL query string.
        if (string.IsNullOrWhiteSpace(url) ||
            Uri.IsWellFormedUriString(url, UriKind.Absolute) is false)
            return Results.BadRequest($"""
                                       The URL query string is required and needs to be well formed.
                                       Consider, ${host}/shorten?url=https://www.microsoft.com.
                                       """);

        // Create a unique, short ID
        var shortenedRouteSegment = Guid.NewGuid().GetHashCode().ToString("X");

        // Create and persist a grain with the shortened ID and full URL
        var shortenerGrain =
            grains.GetGrain<IUrlShortenerGrain>(shortenedRouteSegment);

        await shortenerGrain.SetUrl(url);

        // Return the shortened URL for later use
        var resultBuilder = new UriBuilder(host)
        {
            Path = $"/go/{shortenedRouteSegment}"
        };

        return Results.Ok(resultBuilder.Uri);
    });

app.MapGet("/go/{shortenedRouteSegment:required}",
    static async (IGrainFactory grains, string shortenedRouteSegment) =>
    {
        // Retrieve the grain using the shortened ID and url to the original URL
        var shortenerGrain =
            grains.GetGrain<IUrlShortenerGrain>(shortenedRouteSegment);

        var url = await shortenerGrain.GetUrl();

        // Handles missing schemes, defaults to "http://".
        var redirectBuilder = new UriBuilder(url);

        return Results.Redirect(redirectBuilder.Uri.ToString());
    });

app.Run();