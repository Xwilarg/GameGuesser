namespace GameGuesser.Backend.Database.Interfaces;

public interface IHttpHandler
{
    Task<string> GetStringAsync(string url);
}

