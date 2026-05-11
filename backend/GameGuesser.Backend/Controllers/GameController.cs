using GameGuesser.Backend.Models;
using GameGuesser.Backend.Models.Api;
using GameGuesser.Backend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
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

    public GameController(ILogger<GameController> logger, ConfigManager configManager, HttpClient client)
    {
        _logger = logger;
        _configManager = configManager;
        _client = client;
    }

    [HttpGet("info")]
    [ProducesResponseType<int>(200)]
    public async Task<IActionResult> GetInfo()
    {
        var config = _configManager.GetConfig();
        var now = DateTime.UtcNow.ToString("yyyyMMdd");

        if (config.LastUpdate != now)
        {
            var resp = JsonSerializer.Deserialize<SteamGameInfo>(await _client.GetStringAsync("https://store.steampowered.com/api/appdetails?appids=440&l=english"))!.First()!;

            var desc = Regex.Replace(WebUtility.HtmlDecode(Regex.Unescape(resp.Value.Data.DetailedDescription)).Replace("\t", ""), "<[^>]+>", "");

            List<Token> tokens = [];
            StringBuilder currWord = new();
            foreach (var l in desc)
            {

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
        }

        return StatusCode(StatusCodes.Status200OK, 1);
    }
}