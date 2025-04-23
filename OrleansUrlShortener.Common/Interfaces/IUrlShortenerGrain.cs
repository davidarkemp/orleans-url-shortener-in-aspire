using Orleans;

namespace OrleansUrlShortener.Common.Interfaces;

public interface IUrlShortenerGrain : IGrainWithStringKey
{
    Task SetUrl(string fullUrl);

    Task<string> GetUrl();
}