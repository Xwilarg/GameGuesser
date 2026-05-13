namespace GameGuesser.Backend.Models.Responses
{
    public class WordInfo
    {
        public required int[] FoundIndexes { set; get; }
        public required int[] CloseIndexes { set; get; }
    }
}
