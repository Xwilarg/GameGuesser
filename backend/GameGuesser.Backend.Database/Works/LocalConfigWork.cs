using GameGuesser.Backend.Backend.Models;
using GameGuesser.Backend.Database.Context;
using GameGuesser.Backend.Database.Models;
using System.Text.Json;

namespace GameGuesser.Backend.Database.Works;

public static class LocalConfigWork
{
    public static GameConfig GetLocalConfig(SqliteContext ctx, JsonSerializerOptions opt, Language language)
    {
        return JsonSerializer.Deserialize<GameConfig>(ctx.LocalGames.First(x => x.Language == language).Game!, opt)!;
    }
}
