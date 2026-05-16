namespace GameGuesser.Backend.Models
{
    public class Config
    {
        public required string? LastUpdate { set; get; }
        public required int Iteration { set; get; }
        public required GameConfig Game { set; get; }
    }

    public class GameConfig
    {
        public required Token[] Name { set; get; }
        public required Token[] Description { set; get; }
    }

    public class Token
    {
        public required string Word { set; get; }
        public required bool NeedToBeGuessed { set; get; }
        public string[] SimilarWords { set; get; } = [];
        public string[] AcceptedWords { set; get; } = [];
    }
}
