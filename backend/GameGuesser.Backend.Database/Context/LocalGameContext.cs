using GameGuesser.Backend.Database.Models;
using System.ComponentModel.DataAnnotations;

namespace GameGuesser.Backend.Database.Context;

internal class LocalGameContext
{
    [Key] public required Language Language { set; get; }

    public string? SteamApi { set; get; } = null;
    public string? Game { set; get; } = null;
    public string? Verbs { set; get; } = null;

    public int GameId { set; get; } = 0;

    public bool IsUpdating { set; get; } = false;
}
