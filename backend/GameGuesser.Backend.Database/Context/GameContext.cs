using System.ComponentModel.DataAnnotations;

namespace GameGuesser.Backend.Database.Context;

internal class GameContext
{
    [Key] public int Id { set; get; } = 0;

    public string LastUpdate { set; get; } = "";

    public int Iteration { set; get; } = 0;

    public int GameId { set; get; } = 0;

    public bool IsUpdating { set; get; } = false;
}
