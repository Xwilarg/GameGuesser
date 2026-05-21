using GameGuesser.Backend.Backend.Models;
using GameGuesser.Backend.Database.Models;
using GameGuesser.Backend.Database.Works;
using GameGuesser.Backend.Models.Responses;
using GameGuesser.Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Katsis.Intranet.Controllers;

[ApiController]
[Route("/api/")]
public class GameController(ILogger<GameController> logger, ConfigManager configManager, IServiceScopeFactory scopeFactory) : ControllerBase
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
            if (tokens[i].SimilarWords!.Any(x => string.Compare(x, word, true) == 0))
            {
                closeIndexes.Add(new()
                {
                    Index = i,
                    Score = 1f - Array.FindIndex(tokens[i].SimilarWords!, x => string.Compare(x, word, true) == 0) / (float)(tokens[i].SimilarWords!.Length - 1)
                });
            }
        }
        return new()
        {
            FoundIndexes = foundIndexes.ToArray(),
            CloseIndexes = closeIndexes.ToArray()
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

        var lang = LanguageUtils.StringCountryCodeToLanguage(language);
        if (lang == null)
        {
            return StatusCode(StatusCodes.Status400BadRequest, "The language provided is invalid");
        }

        using var scope = scopeFactory.CreateScope();
        word = word.Trim().ToLowerInvariant();

        var config = scope.ServiceProvider.GetRequiredService<LocalConfigWork>().GetLocalConfig(lang.Value);
        
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
        var lang = LanguageUtils.StringCountryCodeToLanguage(language) ?? Language.English;

        var now = DateTime.UtcNow.ToString("yyyyMMdd");
        using var scope = scopeFactory.CreateScope();
        var configWork = scope.ServiceProvider.GetRequiredService<ConfigWork>();
        var localConfigWork = scope.ServiceProvider.GetRequiredService<LocalConfigWork>();

        if (!configWork.IsUpToDate(now) || !localConfigWork.IsUpToDate(lang))
        {
            return StatusCode(StatusCodes.Status200OK, new LoadingGameInfo() {
                Language = LanguageUtils.LanguageToStringCountryCode((lang == Language.English || localConfigWork.IsAvailable(lang)) ? lang : Language.English) ?? throw new NotImplementedException(),
                Progression = await configManager.UpdateAsync(lang, now)
            });
        }

        var config = localConfigWork.GetLocalConfig(lang);
        return StatusCode(StatusCodes.Status200OK, new GameInfo()
        {
            Language = LanguageUtils.LanguageToStringCountryCode((lang == Language.English || localConfigWork.IsAvailable(lang)) ? lang : Language.English) ?? throw new NotImplementedException(),
            Iteration = configWork.GetIteration(),
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