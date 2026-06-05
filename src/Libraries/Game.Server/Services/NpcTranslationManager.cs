using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace QuantumCore.Game.Services;

public interface INpcTranslationManager
{
    /// <summary>
    /// Returns the translated NPC name for the given vnum and locale,
    /// or null if no translation exists (caller should fall back to proto name).
    /// </summary>
    string? GetName(uint vnum, string locale);
}

public class NpcTranslationManager : INpcTranslationManager
{
    private readonly IFileProvider _fileProvider;
    private readonly ILogger<NpcTranslationManager> _logger;

    // locale → (vnum → name)
    private readonly ConcurrentDictionary<string, IReadOnlyDictionary<uint, string>> _cache = new();

    public NpcTranslationManager(IFileProvider fileProvider, ILogger<NpcTranslationManager> logger)
    {
        _fileProvider = fileProvider;
        _logger = logger;
    }

    public string? GetName(uint vnum, string locale)
    {
        var table = _cache.GetOrAdd(locale, LoadLocale);
        return table.TryGetValue(vnum, out var name) ? name : null;
    }

    private IReadOnlyDictionary<uint, string> LoadLocale(string locale)
    {
        var file = _fileProvider.GetFileInfo($"locale/{locale}.json");
        if (!file.Exists)
        {
            _logger.LogDebug("No NPC translation file for locale '{Locale}'", locale);
            return new Dictionary<uint, string>();
        }

        try
        {
            using var stream = file.CreateReadStream();
            var raw = JsonSerializer.Deserialize<Dictionary<string, string>>(stream) ?? new();
            var result = new Dictionary<uint, string>(raw.Count);
            foreach (var (key, value) in raw)
            {
                if (uint.TryParse(key, out var vnum))
                    result[vnum] = value;
            }
            _logger.LogInformation("Loaded {Count} NPC name translations for locale '{Locale}'", result.Count, locale);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load NPC translation file for locale '{Locale}'", locale);
            return new Dictionary<uint, string>();
        }
    }
}
