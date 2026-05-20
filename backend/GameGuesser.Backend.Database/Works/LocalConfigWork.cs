using GameGuesser.Backend.Backend.Models;
using GameGuesser.Backend.Database.Context;
using GameGuesser.Backend.Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace GameGuesser.Backend.Database.Works;

public class LocalConfigWork(SqliteContext ctx, JsonSerializerOptions opt)
{
    private LocalGameContext GetConfig(Language lang)
        => ctx.LocalGames.First(x => x.Language == lang);

    /// <summary>
    /// Is a game language data initialized yet
    /// </summary>
    public bool IsUpToDate(Language language)
    {
        return GetConfig(language).Game != null;
    }

    /// <summary>
    /// Get the game configuration of a game
    /// </summary>
    public GameConfig GetLocalConfig(Language language)
    {
        return JsonSerializer.Deserialize<GameConfig>(GetConfig(language).Game!, opt)!;
    }

    /// <summary>
    /// Clear all local data, used when a new day happen
    /// </summary>
    public void ClearAllLocalConfig()
    {
        foreach (var local in ctx.LocalGames)
        {
            local.Game = null;
            local.SteamApi = null;
        }
        ctx.SaveChanges();
    }

    public string[][] GetVerbs(Language language)
    {
        var verbs = GetConfig(language).Verbs;
        if (verbs == null) return [];
        return JsonSerializer.Deserialize<string[][]>(verbs, opt)!;
    }

    public void SetSteamAnswer(Language language, string? steamJson)
    {
        GetConfig(language).SteamApi = steamJson;
        ctx.SaveChanges();
    }

    public string? GetSteamAnswer(Language language)
    {
        return GetConfig(language).SteamApi;
    }

    public void SetGameConfig(Language language, GameConfig game)
    {
        GetConfig(language).Game = JsonSerializer.Serialize(game, opt);
        ctx.SaveChanges();
    }

    public bool IsUpdating(Language language)
    {
        return GetConfig(language).IsUpdating;
    }

    public bool ToggleUpdateFlag(Language language, bool value)
    {
        var rows = ctx.LocalGames
            .Where(g => g.Language == language && g.IsUpdating == !value)
            .ExecuteUpdate(s => s.SetProperty(g => g.IsUpdating, value));
        return rows > 0;
    }
}
