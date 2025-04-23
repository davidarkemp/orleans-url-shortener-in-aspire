# Hosting the [Orleans][Orleans] UrlShortener with [Aspire][Aspire]

* Uses Redis as both the cluster and grain store for Orleans.
* Separate Client and Server processes for Orleans.

## Running

```
dotnet aspire run --project .\OrleansUrlShortener.Aspire.AppHost\OrleansUrlShortener.Aspire.AppHost.csproj 
```

You can then test the service by doing something like:

``` curl https://localhost:7225/shorten?url=https://x2764te.ch```

Visiting the response will take you to the shorted url

[Orleans]: https://learn.microsoft.com/en-us/dotnet/orleans/
[Aspire]: https://learn.microsoft.com/en-us/dotnet/aspire/
