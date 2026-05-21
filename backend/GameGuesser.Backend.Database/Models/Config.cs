namespace GameGuesser.Backend.Backend.Models
{
    public class GameConfig
    {
        public required Token[] Name { set; get; }
        public required Token[] Description { set; get; }
        public required Token[] ShortDescription { set; get; }
    }

    public class Token
    {
        public required string Word { set; get; }
        public required bool NeedToBeGuessed { set; get; }
        public string[]? SimilarWords { set; get; } = [];
        public string[]? AcceptedWords { set; get; } = [];
    }
}
