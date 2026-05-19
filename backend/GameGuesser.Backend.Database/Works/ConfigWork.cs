using GameGuesser.Backend.Database.Context;

namespace GameGuesser.Backend.Database.Works;

public static class ConfigWork
{
    public static bool IsUpToDate(SqliteContext ctx, string now)
    {
        return ctx.Game.First().LastUpdate != now;
    }

    public static int GetIteration(SqliteContext ctx)
    {
        return ctx.Game.First().Iteration;
    }
}
