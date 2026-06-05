using CommandLine;
using QuantumCore.API.Game;
using QuantumCore.API.Game.Types.Entities;

namespace QuantumCore.Game.Commands;

[Command("stat", "Adds a status point (takes EPoint name, e.g. ST/HT/DX/IQ)")]
[CommandNoPermission]
public class StatCommand : ICommandHandler<StatCommandOptions>
{
    private const int StatMax = 99;

    public Task ExecuteAsync(CommandContext<StatCommandOptions> context)
    {
        var available = (int)context.Player.GetPoint(EPoint.STATUS_POINTS);
        if (available <= 0)
            return Task.CompletedTask;

        var current = (int)context.Player.GetPoint(context.Arguments.Point);
        if (current >= StatMax)
            return Task.CompletedTask;

        context.Player.AddPoint(context.Arguments.Point, 1);
        context.Player.AddPoint(EPoint.STATUS_POINTS, -1);
        context.Player.SendPoints();
        return Task.CompletedTask;
    }
}

public class StatCommandOptions
{
    [Value(0)] public EPoint Point { get; set; }
}
