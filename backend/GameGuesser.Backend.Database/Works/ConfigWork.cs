using GameGuesser.Backend.Database.Context;
using Microsoft.EntityFrameworkCore;

namespace GameGuesser.Backend.Database.Works;

public class ConfigWork(SqliteContext ctx)
{
    public bool IsUpToDate(string now)
    {
        return ctx.Game.First().LastUpdate == now;
    }

    public int GetIteration()
    {
        return ctx.Game.First().Iteration;
    }

    public void SetGameId(int gameId, string now)
    {
        var g = ctx.Game.First();
        g.GameId = gameId;
        g.LastUpdate = now;
        ctx.SaveChanges();
    }

    public bool ToggleUpdateFlag(bool value)
    {
        var rows = ctx.Game
            .Where(g => g.IsUpdating == !value)
            .ExecuteUpdate(s => s.SetProperty(g => g.IsUpdating, value));
        return rows > 0;
    }
}
