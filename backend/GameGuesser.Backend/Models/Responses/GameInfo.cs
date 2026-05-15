namespace GameGuesser.Backend.Models.Responses
{
    public class GameInfo
    {
        public required int Iteration { set; get; }
        public required GameToken[] Name { set; get; }
        public required GameToken[] Description { set; get; }
    }

    public class GameToken
    {
        public required int Length { set; get; }
        public required string? DisplayedWord { set; get; }
    }
}
