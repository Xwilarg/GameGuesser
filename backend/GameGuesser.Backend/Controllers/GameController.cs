using GameGuesser.Backend.Models.Responses;
using GameGuesser.Backend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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


    [HttpGet("validate/{word}")]
    [ProducesResponseType<WordInfo>(400)]
    public async Task<IActionResult> ValidateWord(string word)
    {
        if (string.IsNullOrWhiteSpace(word))
        {
            return StatusCode(StatusCodes.Status400BadRequest);
        }

        word = word.Trim();

        var config = _configManager.GetConfig();
        List<int> foundIndexes = new();
        List<int> closeIndexes = new();
        for (int i = 0; i < config.Game.Description.Length; i++)
        {
            if (string.Compare(config.Game.Description[i].Word, word, true) == 0)
            {
                foundIndexes.Add(i);
            }
            if (config.Game.Description[i].SimilarWords.Any(x => string.Compare(x, word, true) == 0))
            {
                closeIndexes.Add(i);
            }
        }
        return StatusCode(StatusCodes.Status200OK, new WordInfo()
        {
            FoundIndexes = foundIndexes.ToArray(),
            CloseIndexes = closeIndexes.ToArray()
        });
    }

    [HttpGet("info")]
    [ProducesResponseType<GameInfo>(200)]
    [ProducesResponseType(204)]
    public async Task<IActionResult> GetInfo()
    {
        var config = _configManager.GetConfig();
        var now = DateTime.UtcNow.ToString("yyyyMMdd");

        if (config.LastUpdate != now)
        {
            if (_configManager.IsUpdating) return StatusCode(StatusCodes.Status204NoContent);

            _configManager.Update();
            return StatusCode(StatusCodes.Status204NoContent);
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