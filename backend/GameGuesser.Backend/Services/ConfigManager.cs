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
    public ConfigManager(JsonSerializerOptions options, HttpClient client, JsonSerializerOptions jsonOpt)
    {
        _options = options;
        _client = client;
        _jsonOpt = jsonOpt;
    }

    private JsonSerializerOptions _options;
    private HttpClient _client;
    private readonly JsonSerializerOptions _jsonOpt;
    public bool IsUpdating { set; get; } = false;

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
                    Name = "",
                },
                Iteration = 0,
                LastUpdate = null
            };
    }

    public void WriteConfig(Config config)
    {
        File.WriteAllText(Path, JsonSerializer.Serialize(config, _options));
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
                var resp = JsonSerializer.Deserialize<SteamGameInfo>(_client.GetStringAsync("https://store.steampowered.com/api/appdetails?appids=440&l=english").GetAwaiter().GetResult(), new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                })!.First()!;

                var desc = Regex.Replace(WebUtility.HtmlDecode(Regex.Unescape(resp.Value.Data.DetailedDescription)).Replace("\t", ""), "<[^>]+>", "");

                // Parse description into tokens
                List<Token> tokens = [];
                StringBuilder currWord = new();
                foreach (var l in desc)
                {
                    if (char.IsLetterOrDigit(l))
                    {
                        currWord.Append(l);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(currWord.ToString()))
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
                if (!string.IsNullOrEmpty(currWord.ToString()))
                {
                    tokens.Add(new()
                    {
                        NeedToBeGuessed = true,
                        Word = currWord.ToString()
                    });
                }

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

                config = new()
                {
                    Game = new()
                    {
                        Name = resp.Value.Data.Name,
                        Description = tokens.ToArray(),
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