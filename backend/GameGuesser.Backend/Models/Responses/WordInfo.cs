namespace GameGuesser.Backend.Models.Responses
{
    public class WordInfo
    {
        public required WordBlockInfo Name { set; get; }
        public required WordBlockInfo Description { set; get; }
        public required WordBlockInfo ShortDescription { set; get; }
    }

    public class WordBlockInfo
    {
        public required WordFoundInfo[] FoundIndexes { set; get; }
        public required WordIndexScoreInfo[] CloseIndexes { set; get; }
    }

    public class WordFoundInfo
    {
        public required int Index { set; get; }
        public required string Word { set; get; } // Corrected version of the word
    }

    public class WordIndexScoreInfo
    {
        public required int Index { set; get; }
        public required float Score { set; get; } // How close the word is
    }
}
