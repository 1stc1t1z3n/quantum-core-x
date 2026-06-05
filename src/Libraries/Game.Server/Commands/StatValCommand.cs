using CommandLine;
using QuantumCore.API.Game;
using QuantumCore.API.Game.Types.Entities;

namespace QuantumCore.Game.Commands;

// Client sends "/stat_val st|ht|dx|iq [count]" from the character window + button.
// Normal click: count=1. Ctrl+click: count=10.
[Command("stat_val", "Adds status points via client stat name (st/ht/dx/iq)")]
[CommandNoPermission]
public class StatValCommand : ICommandHandler<StatValCommandOptions>
{
    private const int StatMax = 99;

    private static readonly Dictionary<string, EPoint> StatMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["st"] = EPoint.ST,
        ["ht"] = EPoint.HT,
        ["dx"] = EPoint.DX,
        ["iq"] = EPoint.IQ,
    };

    public Task ExecuteAsync(CommandContext<StatValCommandOptions> context)
    {
        if (!StatMap.TryGetValue(context.Arguments.StatName, out var point))
            return Task.CompletedTask;

        var available = (int)context.Player.GetPoint(EPoint.STATUS_POINTS);
        if (available <= 0)
            return Task.CompletedTask;

        var current = (int)context.Player.GetPoint(point);
        if (current >= StatMax)
            return Task.CompletedTask;

        var count = Math.Clamp(context.Arguments.Count, 1, 10);
        count = Math.Min(count, available);
        count = Math.Min(count, StatMax - current);

        context.Player.AddPoint(point, count);
        context.Player.AddPoint(EPoint.STATUS_POINTS, -count);
        context.Player.SendPoints();
        return Task.CompletedTask;
    }
}

public class StatValCommandOptions
{
    [Value(0)] public string StatName { get; set; } = "";
    [Value(1)] public int Count { get; set; } = 1;
}
