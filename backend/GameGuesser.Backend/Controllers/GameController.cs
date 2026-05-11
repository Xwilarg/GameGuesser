using GameGuesser.Backend.Models;
using GameGuesser.Backend.Models.Api;
using GameGuesser.Backend.Models.Responses;
using GameGuesser.Backend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Katsis.Intranet.Controllers;

[ApiController]
[Route("/api/")]
public class GameController : ControllerBase
{
    private readonly ILogger<GameController> _logger;
    private readonly ConfigManager _configManager;
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOpt;

    public GameController(ILogger<GameController> logger, ConfigManager configManager, HttpClient client, JsonSerializerOptions jsonOpt)
    {
        _logger = logger;
        _configManager = configManager;
        _client = client;
        _jsonOpt = jsonOpt;
    }

    [HttpGet("info")]
    [ProducesResponseType<GameInfo>(200)]
    public async Task<IActionResult> GetInfo()
    {
        var config = _configManager.GetConfig();
        var now = DateTime.UtcNow.ToString("yyyyMMdd");

        if (config.LastUpdate != now)
        {
            lock(_configManager.Lock) // Prevent 2 users to reload daily word at the same time
            {
                config = _configManager.GetConfig();
                now = DateTime.UtcNow.ToString("yyyyMMdd");

                if (config.LastUpdate != now)
                {
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
                    _configManager.WriteConfig(config);
                }
            }
        }

        return StatusCode(StatusCodes.Status200OK, new GameInfo()
        {
            Tokens = config.Game.Description.Select(x => new GameToken()
            {
                DisplayedWord = x.NeedToBeGuessed ? null : x.Word,
                Length = x.Word.Length
            }).ToArray()
        });
    }
}