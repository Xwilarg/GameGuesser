using GameGuesser.Backend.Interfaces;

namespace GameGuesser.Backend.Services;

public class HttpClientHandler : IHttpHandler
{
    private HttpClient _client = new();

    public async Task<string> GetStringAsync(string url)
    {
        return await _client.GetStringAsync(url);
    }
}
