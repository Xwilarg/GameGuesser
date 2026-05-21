using GameGuesser.Backend.Database.Context;
using GameGuesser.Backend.Database.Interfaces;
using GameGuesser.Backend.Database.Models;

namespace GameGuesser.Backend.Database.Queries;

public class InitWork(SqliteContext ctx)
{
    public async Task<string?> GetVerbsJsonAsync(IHttpHandler client, Language lang)
    {
        if (lang == Language.English)
        {
            return await client.GetStringAsync("https://raw.githubusercontent.com/monolithpl/verb.forms.dictionary/refs/heads/master/json/verbs-dictionaries.json");
        }
        return null;
    }

    public async Task InitAsync(IHttpHandler client)
    {
        if (!ctx.Game.Any())
        {
            ctx.Game.Add(new());
            ctx.SaveChanges();
        }

        ctx.Game.First().IsUpdating = false;
        foreach (var local in ctx.LocalGames)
        {
            local.IsUpdating = false;
        }

        foreach (var lang in Enum.GetValues<Language>().Cast<Language>())
        {
            var local = ctx.LocalGames.FirstOrDefault(x => x.Language == lang);
            if (local == null)
            {
                ctx.LocalGames.Add(new()
                {
                    Language = lang,
                    Verbs = await GetVerbsJsonAsync(client, lang)
                });
            }
            else if (local.Verbs == null)
            {
                var json = await GetVerbsJsonAsync(client, lang);
                local.Verbs = json;
            }
        }

        ctx.SaveChanges();
    }
}
