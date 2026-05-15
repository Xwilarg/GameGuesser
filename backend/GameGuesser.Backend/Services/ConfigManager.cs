using GameGuesser.Backend.Models;
using GameGuesser.Backend.Models.Api;
using GameGuesser.Backend.Models.Responses;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace GameGuesser.Backend.Services;

/// <summary>
/// Store all connections
/// </summary>
public class ConfigManager
{
    public ConfigManager(JsonSerializerOptions options, HttpClient client, JsonSerializerOptions jsonOpt, Random rand)
    {
        _options = options;
        _client = client;
        _jsonOpt = jsonOpt;
        _rand = rand;
    }

    private JsonSerializerOptions _options;
    private HttpClient _client;
    private readonly JsonSerializerOptions _jsonOpt;
    private Random _rand;
    public bool IsUpdating { set; get; } = false;

    public int[] Games = // Placeholder list, until I have a batter system to enter games
        [730, 570, 4000, 440, 620, 550, 546560, 400, 220, 240, 70, 10, 500, 300, 50, 130, 20, 30, 60, 40, 1046930, 300, 1840, 320, 450390, 1902490, 868020, 630, 80, 360, 583950, 306130, 1151340, 1716740, 489830, 377160, 2677660, 2623190, 22380, 588430, 3017860, 205100, 782330, 22370, 614570, 480490, 403640, 379720, 22320, 1252330, 900883, 22330, 38420, 611670, 612880, 201810, 3286930, 268050, 22300, 2320, 601430, 9200, 9010, 350080, 208200, 38400, 2270, 611660, 38410, 2310, 1475810, 1056960, 1148590, 359550, 2842040, 3274580, 291550, 2221490, 3751950, 2840770, 304390, 2853730, 3159330, 1866130, 2698940, 916440, 2225070, 812140, 2231380, 3918850, 2208920, 289650, 242050, 2369390, 582160, 552520, 235600, 460930, 3035570, 646910, 220240, 2556990, 221680, 242550, 311560, 447040, 2717880, 213670, 33230, 1281630, 48190, 365590, 488790, 15100, 243470, 298110, 201870, 19900, 368500, 1172470, 2807960, 3405690, 1286830, 1222670, 3028330, 2001120, 1426210, 2995920, 3059520, 1289670, 2669320, 1237950, 1774580, 690790, 1172380, 1222700, 1238810, 3314070, 47890, 3354750, 1693980, 1238840, 1328670, 1849250, 1846380, 1225570, 1237970, 1222680, 17390, 3654560, 19680, 3230400, 1262560, 1845910, 1922560, 24780, 3314060, 1213210, 1517290, 1238860, 1086940, 435150, 373420, 219780, 214170, 243950, 230230, 219760, 3472040, 1285190, 289070, 1295660, 3717070, 8930, 1941540, 268500, 368260, 2385530, 397540, 49520, 1286680, 8870, 1030840, 7670, 200510, 1030830, 2878960, 16810, 360430, 65980, 261640, 8850, 882100, 294100, 281990, 1158310, 949230, 394360, 1669000, 529340, 236850, 3450310, 255710, 983870, 1385380, 3215050, 1324130, 532790, 233450, 203770, 637090, 1145350, 1145360, 237930, 107100, 462770, 1466860, 2483190, 1172620, 813780, 2537590, 1934680, 1551360, 2661300, 962130, 1250410, 976730, 933110, 2440510, 1672970, 288470, 2457220, 495420, 1057090, 1449110, 2627260, 261570, 2461850, 578650, 1017900, 291650, 2523720, 1240440, 1934570, 314160, 1205520, 560130, 459220, 719040, 1097840, 3041230, 2344520, 2536520, 1771300, 379430, 412020, 934700, 286690, 287390, 794260, 1451190, 55230, 978300, 742420, 206420, 2821610, 1939970, 383150, 990380, 826630, 667720, 301910, 9480, 1153640, 383180, 20530, 223100, 230410, 29900, 4095380, 3321460, 582660, 1174180, 3240220, 2668510, 1547000, 12210, 110800, 12200, 204100, 1404210, 12130, 202570, 722230, 1546990, 12220, 1546970, 553850, 1659420, 3280350, 2651280, 2531310, 1888930, 2215430, 1817070, 2322010, 1259420, 2420110, 1593500, 1817190, 1895880, 1649240, 2561580, 2172010, 1151640, 1599660, 394510, 2698870, 2428810, 3378960, 417880, 3787240, 105600, 342640, 3241660, 1304680, 264710, 848450, 1962700, 4920, 4900, 310110, 39210, 2624870, 1462040, 2909400, 1290000, 2552430, 2833580, 2515020, 524220, 1004640, 637650, 1113560, 319630, 2893570, 921570, 2499860, 2484110, 1971650, 3014320, 595520, 359870, 1874000, 1451090, 1295510, 1265920, 2455640, 2175540, 1072420, 1173820, 2701440, 613830, 1608070, 307690, 377840, 292120, 225540, 1358700, 2552450, 532210, 680420, 292140, 1026680, 2436570, 1850510, 1173810, 2106840, 1446650, 1173800, 2238900, 1091500, 292030, 20920, 1284410, 20900, 973760, 739650, 1711950, 318600, 303800, 2684660, 3357650, 3764200, 601150, 1364780, 2050650, 631510, 2246340, 2852190, 952060, 883710, 418370, 339340, 582010, 2054970, 1196590, 4249120, 4249110, 2, 1446780, 304240, 4249100, 787480, 3500390, 2356560, 287290, 4249150, 1158850, 2401970, 1277400, 4249140, 2187220, 2527390, 310950, 2400430, 4249130, 2510710, 1755910, 587620, 2694490, 238960, 413150, 1973530, 568220, 1256670];

    private string Path
        => Debugger.IsAttached || Environment.GetEnvironmentVariable("TEST") == "1" ? "info.json" : "/data/info.json";

    public Config GetConfig()
    {
        return File.Exists(Path)
            ? JsonSerializer.Deserialize<Config>(File.ReadAllText(Path), _options)!
            : new()
            {
                Game = new()
                {
                    Description = [],
                    Name = [],
                },
                Iteration = 0,
                LastUpdate = null
            };
    }

    public void WriteConfig(Config config)
    {
        File.WriteAllText(Path, JsonSerializer.Serialize(config, _options));
    }

    private Token[] StringToTokens(string text)
    {
        List<Token> tokens = [];
        StringBuilder currWord = new();
        foreach (var l in text)
        {
            if (char.IsLetterOrDigit(l))
            {
                currWord.Append(l); // Making a word
            }
            else // We found some punctuation or a blank space
            {
                if (!string.IsNullOrEmpty(currWord.ToString())) // We stored letters before so we have a pending word
                {
                    tokens.Add(new()
                    {
                        NeedToBeGuessed = true,
                        Word = currWord.ToString()
                    });
                    currWord = new();
                }
                tokens.Add(new()
                {
                    NeedToBeGuessed = false,
                    Word = l.ToString()
                });
            }
        }
        if (!string.IsNullOrEmpty(currWord.ToString())) // String ends with a pending word
        {
            tokens.Add(new()
            {
                NeedToBeGuessed = true,
                Word = currWord.ToString()
            });
        }

        // For each word we use an API to detect which words are close in meaning
        Dictionary<string, string[]> adjacents = [];
        foreach (var token in tokens)
        {
            if (token.NeedToBeGuessed)
            {
                if (!adjacents.ContainsKey(token.Word.ToLowerInvariant()))
                {
                    adjacents.Add(token.Word.ToLowerInvariant(), JsonSerializer.Deserialize<SimilarInfo[]>(_client.GetStringAsync($"https://api.datamuse.com/words?ml={token.Word.ToLowerInvariant()}").GetAwaiter().GetResult(), _jsonOpt)!.Select(x => x.Word).ToArray());
                }

                token.SimilarWords = adjacents[token.Word.ToLowerInvariant()];
            }
        }
        return tokens.ToArray();
    }

    public void Update()
    {
        Task.Run(() =>
        {
            try
            {
                var config = GetConfig();
                var now = DateTime.UtcNow.ToString("yyyyMMdd");

                // Get game data
                var resp = JsonSerializer.Deserialize<SteamGameInfo>(_client.GetStringAsync($"https://store.steampowered.com/api/appdetails?appids={Games[_rand.Next(0, Games.Length)]}&l=english").GetAwaiter().GetResult(), new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                })!.First()!;

                var desc = Regex.Replace(WebUtility.HtmlDecode(Regex.Unescape(resp.Value.Data.DetailedDescription)).Replace("\t", ""), "<[^>]+>", "");

                // Parse description into tokens
                var tokensDesc = StringToTokens(desc);
                var tokensName = StringToTokens(resp.Value.Data.Name);

                config = new()
                {
                    Game = new()
                    {
                        Name = tokensName,
                        Description = tokensDesc,
                    },
                    Iteration = config.Iteration + 1,
                    LastUpdate = now
                };
                WriteConfig(config);
            }
            finally
            {
                IsUpdating = false;
            }
        });
    }
}