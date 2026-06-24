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

    public required SteamGameGenreInfo[] Genres { set; get; }
    public required SteamGameMovieInfo[] Movies { set; get; }
    public required SteamGameScreenshotInfo[] Screenshots { set; get; }
}

public class SteamGameGenreInfo
{
    public required string Description { set; get; }
}

public class SteamGameMovieInfo
{
    public required string HlsH264 { set; get; }
}

public class SteamGameScreenshotInfo
{
    public required string PathFull { set; get; }
}