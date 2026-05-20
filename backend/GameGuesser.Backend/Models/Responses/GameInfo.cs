namespace GameGuesser.Backend.Models.Responses
{
    public class LoadingGameInfo
    {
        public bool IsReady => false;
        public required string Language { set; get; }
        public required int Progression { set; get; }
    }

    public class GameInfo
    {
        public bool IsReady => true;
        public required int Iteration { set; get; }
        public required GameToken[] Name { set; get; }
        public required GameToken[] ShortDescription { set; get; }
        public required GameToken[] Description { set; get; }
    }

    public class GameToken
    {
        public required int Length { set; get; }
        public required string? DisplayedWord { set; get; }
    }
}
