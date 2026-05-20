using FluentAssertions;
using GameGuesser.Backend.Backend.Models;
using GameGuesser.Backend.Database.Interfaces;
using GameGuesser.Backend.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace GameGuesser.Backend.Test
{
    public class WordHttpClient : IHttpHandler
    {
        public async Task<string> GetStringAsync(string url)
        {
            return """
                              [{
                  "word": "test",
                  "score": 39972497,
                  "tags": []
                }]
                """;
        }
    }

    public class ParsingTests
    {
        private ConfigManager _config;
        private string[][] _verbs;

        [SetUp]
        public async Task Setup()
        {
            Environment.SetEnvironmentVariable("TEST", "1");
            var serviceCollection = new ServiceCollection();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            _config = new(new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }, new WordHttpClient(), serviceProvider.GetRequiredService<IServiceScopeFactory>());

            using var client = new HttpClient();
            _verbs = JsonSerializer.Deserialize<string[][]>(await client.GetStringAsync("https://raw.githubusercontent.com/monolithpl/verb.forms.dictionary/refs/heads/master/json/verbs-dictionaries.json"),
                new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                })!;
        }

        [TestCase(
            "The Flagship Turn-Based Strategy Game Returns\u003Cbr\u003E\u003Cbr\u003EBecome Ruler of the World by establishing and leading a civilization from the dawn of man into the space age",
            "The Flagship Turn-Based Strategy Game Returns\n\nBecome Ruler of the World by establishing and leading a civilization from the dawn of man into the space age"
         )]
        [TestCase(
            "\u003Cp class=\"bb_paragraph\" \u003E\u003Cstrong\u003E&quot;The most fun you can have online&quot;\u003C/strong\u003E - PC Gamer\u003C/p\u003E\u003Cp class=\"bb_paragraph\" \u003E\u003C/p\u003E\u003Cp class=\"bb_paragraph\" \u003E\t\t\t\t\t\t\t\t\t \u003Cstrong\u003EIs now FREE!",
            "\"The most fun you can have online\" - PC Gamer Is now FREE!"
         )]
        [TestCase(
            "Dig, Fight, Explore, Build:  The very world is at your fingertips as you fight for survival, fortune, and glory.   Will you delve deep into cavernous expanses in search of treasure and raw materials with which to craft ever-evolving gear, machinery, and aesthetics?",
            "Dig, Fight, Explore, Build: The very world is at your fingertips as you fight for survival, fortune, and glory. Will you delve deep into cavernous expanses in search of treasure and raw materials with which to craft ever-evolving gear, machinery, and aesthetics?"
        )]
        [TestCase(
            "\u003Ch1\u003EDigital Deluxe Edition\u003C/h1\u003E\u003Cp\u003E\u003Cp class=\"bb_paragraph\" \u003E\u003Cspan class=\"bb_img_ctn\"\u003E\u003Cimg class=\"bb_img\" src=\"https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/3321460/extras/8ae4dca408143f078562cf8345b7e69f.avif?t=1777016399\" width=1024 height=1024 /\u003E\u003C/span\u003E\u003C/p\u003E\u003Cp class=\"bb_paragraph\" style=\"text-align: center\"\u003E  \u003C/p\u003E\u003Cp class=\"bb_paragraph\" style=\"text-align: center\"\u003E\u003Cspan class=\"bb_img_ctn\"\u003E\u003Cimg class=\"bb_img\" src=\"https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/3321460/extras/c092a80d34c46c72333596218d7438cb.avif?t=1777016399\" width=1560 height=100 /\u003E\u003C/span\u003E\u003C/p\u003E\u003Cp class=\"bb_paragraph\" style=\"text-align: center\"\u003EWant to add a little more grandeur to your journey in Pywel?​\u003C/p\u003E\u003Cp class=\"bb_paragraph\" style=\"text-align: center\"\u003EThe Crimson Desert Deluxe Edition includes the full game and the following exclusive items",
            "Digital Deluxe Edition Want to add a little more grandeur to your journey in Pywel? The Crimson Desert Deluxe Edition includes the full game and the following exclusive items"
        )]
        public void TestDecodeHtml(string html, string decoded)
        {
            _config.DecodeHtml(html).Should().Be(decoded);
        }

        [Test]
        public async Task TestTokenParsing()
        {
            var tokens = await _config.StringToTokensAsync("a great journey to drink water, with you", _verbs);
            tokens.Should().BeEquivalentTo([
                new Token()
                {
                    Word = "a",
                    NeedToBeGuessed = true,
                    SimilarWords = [ "test" ],
                    AcceptedWords = [ "a", "as" ]
                },
                new Token()
                {
                    Word = " ",
                    NeedToBeGuessed = false
                },
                new Token()
                {
                    Word = "great",
                    NeedToBeGuessed = true,
                    SimilarWords = [ "test" ],
                    AcceptedWords = [ "great", "greats" ]
                },
                new Token()
                {
                    Word = " ",
                    NeedToBeGuessed = false
                },
                new Token()
                {
                    Word = "journey",
                    NeedToBeGuessed = true,
                    SimilarWords = [ "test" ],
                    AcceptedWords = [ "journey", "journeys", "journeying", "journeyed" ]
                },
                new Token()
                {
                    Word = " ",
                    NeedToBeGuessed = false
                },
                new Token()
                {
                    Word = "to",
                    NeedToBeGuessed = true,
                    SimilarWords = [ "test" ],
                    AcceptedWords = [ "to", "tos" ]
                },
                new Token()
                {
                    Word = " ",
                    NeedToBeGuessed = false
                },
                new Token()
                {
                    Word = "drink",
                    NeedToBeGuessed = true,
                    SimilarWords = [ "test" ],
                    AcceptedWords = [ "drink", "drank", "drunk", "drinks", "drinking" ]
                },
                new Token()
                {
                    Word = " ",
                    NeedToBeGuessed = false
                },
                new Token()
                {
                    Word = "water",
                    NeedToBeGuessed = true,
                    SimilarWords = [ "test" ],
                    AcceptedWords = [ "water", "waters", "watered", "watering" ]
                },
                new Token()
                {
                    Word = ",",
                    NeedToBeGuessed = false
                },
                new Token()
                {
                    Word = " ",
                    NeedToBeGuessed = false
                },
                new Token()
                {
                    Word = "with",
                    NeedToBeGuessed = true,
                    SimilarWords = [ "test" ],
                    AcceptedWords = [ "with", "withs" ]
                },
                new Token()
                {
                    Word = " ",
                    NeedToBeGuessed = false
                },
                new Token()
                {
                    Word = "you",
                    NeedToBeGuessed = true,
                    SimilarWords = [ "test" ],
                    AcceptedWords = [ "you", "yous" ]
                },
            ]);
        }
    }
}
