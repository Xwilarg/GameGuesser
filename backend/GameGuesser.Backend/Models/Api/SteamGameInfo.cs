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
    public SteamGameMovieInfo[]? Movies { set; get; }
    public required SteamGameScreenshotInfo[] Screenshots { set; get; }
    public required SteamGameAchievementListInfo Achievements { set; get; }

    public required string[] Developers { set; get; }
    public required string[] Publishers { set; get; }
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

public class SteamGameAchievementListInfo
{
    public required SteamGameAchievementInfo[] Highlighted { set; get; }
}

public class SteamGameAchievementInfo
{
    public required string Path { set; get; }
}