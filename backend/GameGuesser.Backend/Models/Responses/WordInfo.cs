namespace GameGuesser.Backend.Models.Responses
{
    public class WordInfo
    {
        public required WordBlockInfo Name { set; get; }
        public required WordBlockInfo Description { set; get; }
    }

    public class WordBlockInfo
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
