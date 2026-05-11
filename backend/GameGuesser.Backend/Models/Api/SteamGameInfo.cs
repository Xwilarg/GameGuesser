namespace GameGuesser.Backend.Models.Api;

public class SteamGameInfo : Dictionary<string, SteamGameEntryInfo> { }

public class SteamGameEntryInfo
{
    public SteamGameDataInfo Data { set; get; }
}

public class SteamGameDataInfo
{
    public string Type { set; get; }
    public string Name { set; get; }
    public string DetailedDescription { set; get; }
}