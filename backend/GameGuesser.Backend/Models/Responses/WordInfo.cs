namespace GameGuesser.Backend.Models.Responses
{
    public class WordInfo
    {
        public required int[] FoundIndexes { set; get; }
        public required WordIndexScoreInfo[] CloseIndexes { set; get; }
    }

    public class WordIndexScoreInfo
    {
        public int Index { set; get; }
        public float Score { set; get; }
    }
}
