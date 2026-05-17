using GameGuesser.Backend.Interfaces;
using GameGuesser.Backend.Models;
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
    private readonly IHttpHandler _client;
    private readonly JsonSerializerOptions _jsonOpt;

    public GameController(ILogger<GameController> logger, ConfigManager configManager, IHttpHandler client, JsonSerializerOptions jsonOpt)
    {
        _logger = logger;
        _configManager = configManager;
        _client = client;
        _jsonOpt = jsonOpt;
    }

    private WordBlockInfo GetFoundWords(Token[] tokens, string word)
    {
        List<WordFoundInfo> foundIndexes = [];
        List<WordIndexScoreInfo> closeIndexes = [];
        for (int i = 0; i < tokens.Length; i++)
        {
            if (tokens[i].AcceptedWords.Contains(word))
            {
                foundIndexes.Add(new()
                {
                    Word = tokens[i].Word,
                    Index = i
                });
            }
            if (tokens[i].SimilarWords.Any(x => string.Compare(x, word, true) == 0))
            {
                closeIndexes.Add(new()
                {
                    Index = i,
                    Score = 1f - Array.FindIndex(tokens[i].SimilarWords, x => string.Compare(x, word, true) == 0) / (float)(tokens[i].SimilarWords.Length - 1)
                });
            }
        }
        return new()
        {
            FoundIndexes = foundIndexes.ToArray(),
            CloseIndexes = closeIndexes.ToArray()
        };
    }

    [HttpGet("validate/{word}")]
    [ProducesResponseType<WordInfo>(400)]
    public async Task<IActionResult> ValidateWord(string word)
    {
        if (string.IsNullOrWhiteSpace(word))
        {
            return StatusCode(StatusCodes.Status400BadRequest);
        }

        word = word.Trim().ToLowerInvariant();

        var config = _configManager.GetConfig();
        
        return StatusCode(StatusCodes.Status200OK, new WordInfo()
        {
            Name = GetFoundWords(config.Game.Name, word),
            Description = GetFoundWords(config.Game.Description, word),
            ShortDescription = GetFoundWords(config.Game.ShortDescription, word)
        });
    }

    [HttpGet("info")]
    [ProducesResponseType<GameInfo>(200)]
    [ProducesResponseType<LoadingGameInfo>(200)]
    public async Task<IActionResult> GetInfo()
    {
        var config = _configManager.GetConfig();
        var now = DateTime.UtcNow.ToString("yyyyMMdd");

        if (config.LastUpdate != now)
        {
            if (_configManager.IsUpdating)
                return StatusCode(StatusCodes.Status200OK, new LoadingGameInfo() { Progression = _configManager.Progression });

            _configManager.Update();
            return StatusCode(StatusCodes.Status200OK, new LoadingGameInfo() { Progression = 0 });
        }

        return StatusCode(StatusCodes.Status200OK, new GameInfo()
        {
            Iteration = config.Iteration,
            Name = config.Game.Name.Select(x => new GameToken()
            {
                DisplayedWord = x.NeedToBeGuessed ? null : x.Word,
                Length = x.Word.Length
            }).ToArray(),
            Description = config.Game.Description.Select(x => new GameToken()
            {
                DisplayedWord = x.NeedToBeGuessed ? null : x.Word,
                Length = x.Word.Length
            }).ToArray(),
            ShortDescription = config.Game.ShortDescription.Select(x => new GameToken()
            {
                DisplayedWord = x.NeedToBeGuessed ? null : x.Word,
                Length = x.Word.Length
            }).ToArray(),
        });
    }
}