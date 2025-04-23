using Microsoft.Extensions.Hosting;
using OrleansUrlShortener.Common;
using OrleansUrlShortener.Common.Interfaces;

namespace OrleansUrlShortener.Server;

public sealed class UrlShortenerGrain(
    [PersistentState(
        stateName: "url",
        storageName: Names.Grains.Storage.Urls)]
    IPersistentState<UrlDetails> state)
    : Grain, IUrlShortenerGrain
{
    public async Task SetUrl(string fullUrl)
    {
        state.State = new UrlDetails
        {
            ShortenedRouteSegment = this.GetPrimaryKeyString(),
            FullUrl = fullUrl
        };

        await state.WriteStateAsync().ConfigureAwait(false);
    }

    public Task<string> GetUrl()
    {
        return Task.FromResult(state.State.FullUrl);
    }
}