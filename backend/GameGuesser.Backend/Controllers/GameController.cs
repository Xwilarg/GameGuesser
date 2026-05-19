using GameGuesser.Backend.Backend.Models;
using GameGuesser.Backend.Database.Context;
using GameGuesser.Backend.Database.Models;
using GameGuesser.Backend.Database.Works;
using GameGuesser.Backend.Models.Responses;
using GameGuesser.Backend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Katsis.Intranet.Controllers;

[ApiController]
[Route("/api/")]
public class GameController(ILogger<GameController> logger, ConfigManager configManager, SqliteContext ctx, JsonSerializerOptions jsonOpt) : ControllerBase
{
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

    private Language? StringToLanguage(string str)
    {
        return str switch
        {
            "en" => Language.English,
            _ => null
        };
    }

    [HttpGet("validate/{language}/{word}")]
    [ProducesResponseType<WordInfo>(400)]
    public async Task<IActionResult> ValidateWord(string language, string word)
    {
        if (string.IsNullOrWhiteSpace(word))
        {
            return StatusCode(StatusCodes.Status400BadRequest, "Word query parameter is missing");
        }

        var lang = StringToLanguage(language);
        if (lang == null)
        {
            return StatusCode(StatusCodes.Status400BadRequest, "The language provided is invalid");
        }

        word = word.Trim().ToLowerInvariant();

        var config = LocalConfigWork.GetLocalConfig(ctx, jsonOpt, lang.Value);
        
        return StatusCode(StatusCodes.Status200OK, new WordInfo()
        {
            Name = GetFoundWords(config.Name, word),
            Description = GetFoundWords(config.Description, word),
            ShortDescription = GetFoundWords(config.ShortDescription, word)
        });
    }

    [HttpGet("info/{language}")]
    [ProducesResponseType<GameInfo>(200)]
    [ProducesResponseType<LoadingGameInfo>(200)]
    [ProducesResponseType<WordInfo>(400)]
    public async Task<IActionResult> GetInfo(string language)
    {
        var lang = StringToLanguage(language);
        if (lang == null)
        {
            return StatusCode(StatusCodes.Status400BadRequest, "The language provided is invalid");
        }

        var now = DateTime.UtcNow.ToString("yyyyMMdd");

        if (!ConfigWork.IsUpToDate(ctx, now))
        {
            if (configManager.IsUpdating)
                return StatusCode(StatusCodes.Status200OK, new LoadingGameInfo() { Progression = configManager.Progression });

            configManager.Update();
            return StatusCode(StatusCodes.Status200OK, new LoadingGameInfo() { Progression = 0 });
        }

        var config = LocalConfigWork.GetLocalConfig(ctx, jsonOpt, lang.Value);
        return StatusCode(StatusCodes.Status200OK, new GameInfo()
        {
            Iteration = ConfigWork.GetIteration(ctx),
            Name = config.Name.Select(x => new GameToken()
            {
                DisplayedWord = x.NeedToBeGuessed ? null : x.Word,
                Length = x.Word.Length
            }).ToArray(),
            Description = config.Description.Select(x => new GameToken()
            {
                DisplayedWord = x.NeedToBeGuessed ? null : x.Word,
                Length = x.Word.Length
            }).ToArray(),
            ShortDescription = config.ShortDescription.Select(x => new GameToken()
            {
                DisplayedWord = x.NeedToBeGuessed ? null : x.Word,
                Length = x.Word.Length
            }).ToArray(),
        });
    }
}