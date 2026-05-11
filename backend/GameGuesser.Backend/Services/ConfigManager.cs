using GameGuesser.Backend.Models;
using System.Diagnostics;
using System.Text.Json;

namespace GameGuesser.Backend.Services;

/// <summary>
/// Store all connections
/// </summary>
public class ConfigManager
{
    public ConfigManager(JsonSerializerOptions options)
    {
        _options = options;
    }

    private JsonSerializerOptions _options;
    public object Lock { get; } = new();

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
}