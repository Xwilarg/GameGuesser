namespace GameGuesser.Backend.Models.Responses
{
    public class DatamuseSimilarInfo
    {
        public required string Word { set; get; }
    }

    public class FreeDictionarySimilarInfo
    {
        public required FreeDictionarySimilarEntryInfo[] Entries { set; get; }
    }

    public class FreeDictionarySimilarEntryInfo
    {
        public required FreeDictionarySimilarEntryFormInfo[] Forms { set; get; }
        public required FreeDictionarySimilarEntrySenseInfo[] Senses { set; get; }
    }

    public class FreeDictionarySimilarEntryFormInfo
    {
        public required string Word { set; get; }
        public required string[] Tags { set; get; }
    }

    public class FreeDictionarySimilarEntrySenseInfo
    {
        public required string[] Synonyms { set; get; }
    }
}
