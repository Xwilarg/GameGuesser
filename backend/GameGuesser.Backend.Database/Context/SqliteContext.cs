using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace GameGuesser.Backend.Database.Context;

public class SqliteContext : DbContext
{
    internal DbSet<GameContext> Game { set; get; }
    internal DbSet<LocalGameContext> LocalGames { set; get; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite(
            Debugger.IsAttached || AppDomain.CurrentDomain.FriendlyName == "ef" || Environment.GetEnvironmentVariable("TEST") == "1"
            ? $"Data Source=Sqlite.db"
            : $"Data Source=/data/Sqlite.db"
        );
    }
}
