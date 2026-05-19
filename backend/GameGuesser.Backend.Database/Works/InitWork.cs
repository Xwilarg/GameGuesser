using GameGuesser.Backend.Database.Context;

namespace GameGuesser.Backend.Database.Queries;

public static class InitWork
{
    public static async Task InitAsync(SqliteContext ctx, HttpClient client)
    {
        using var transaction = ctx.Database.BeginTransaction();
        ctx.Game.First().IsUpdating = false;
        foreach (var local in ctx.LocalGames)
        {
            local.IsUpdating = false;
        }

        if (ctx.LocalGames.Any(x => x.Language == Models.Language.English))
        {
            var verbs = await client.GetStringAsync("https://raw.githubusercontent.com/monolithpl/verb.forms.dictionary/refs/heads/master/json/verbs-dictionaries.json");
            ctx.LocalGames.Add(new() {
                Language = Models.Language.English,
                Verbs = verbs
            });
        }

        transaction.Commit();
    }
}
