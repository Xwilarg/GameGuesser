namespace GameGuesser.Backend.Models.Responses
{
    public class GameInfo
    {
        public required GameToken[] Tokens { set; get; }
    }

    public class GameToken
    {
        public required int Length { set; get; }
        public required string? DisplayedWord { set; get; }
    }
}
