namespace GameGuesser.Backend.Models.Api;

public class SteamGameInfo : Dictionary<string, SteamGameEntryInfo> { }

public class SteamGameEntryInfo
{
    public required SteamGameDataInfo Data { set; get; }
}

public class SteamGameDataInfo
{
    public required string Type { set; get; }
    public required string Name { set; get; }
    public required string DetailedDescription { set; get; }
    public required string ShortDescription { set; get; }
}