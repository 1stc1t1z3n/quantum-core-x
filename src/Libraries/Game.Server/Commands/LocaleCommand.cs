using CommandLine;
using QuantumCore.API.Game;

namespace QuantumCore.Game.Commands;

[Command("locale", "Set the client locale for NPC name translation")]
[CommandNoPermission]
public class LocaleCommand : ICommandHandler<LocaleCommandOptions>
{
    private static readonly HashSet<string> Allowed = new(StringComparer.OrdinalIgnoreCase)
    {
        "pt", "en", "de", "fr", "es", "it", "cz", "pl", "tr", "ro", "nl", "hu"
    };

    public Task ExecuteAsync(CommandContext<LocaleCommandOptions> context)
    {
        var lang = context.Arguments.Lang.ToLowerInvariant();
        if (Allowed.Contains(lang))
            context.Player.Connection.Locale = lang;
        return Task.CompletedTask;
    }
}

public class LocaleCommandOptions
{
    [Value(0)] public string Lang { get; set; } = "en";
}
