namespace GameGuesser.Backend.Interfaces;

public interface IHttpHandler
{
    Task<string> GetStringAsync(string url);
}

