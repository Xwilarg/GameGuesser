namespace GameGuesser.Backend.Models.Responses
{
    public class GameInfo
    {
        public required GameToken[] Tokens;
    }

    public class GameToken
    {
        public required string Length { set; get; }
        public required string? DisplayedWord { set; get; }
    }
}
