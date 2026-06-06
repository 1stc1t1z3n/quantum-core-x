using CommandLine;
using QuantumCore.API.Game;
using QuantumCore.API.Game.Types.Items;
using QuantumCore.API.Game.World;
using QuantumCore.Game.World.Entities;

namespace QuantumCore.Game.Commands;

[Command("affect", "Add a timed affect to a player: /affect <applyType> <value> [duration] [target]")]
public class AffectAddCommand : ICommandHandler<AffectAddCommandOptions>
{
    private readonly IWorld _world;

    public AffectAddCommand(IWorld world)
    {
        _world = world;
    }

    public Task ExecuteAsync(CommandContext<AffectAddCommandOptions> context)
    {
        var target = context.Player;
        if (!string.IsNullOrWhiteSpace(context.Arguments.Target))
        {
            target = _world.GetPlayer(context.Arguments.Target);
        }

        if (target is not PlayerEntity player)
        {
            context.Player.SendChatMessage("Target not found");
            return Task.CompletedTask;
        }

        if (!Enum.TryParse<EApplyType>(context.Arguments.ApplyType, ignoreCase: true, out var applyType))
        {
            context.Player.SendChatMessage($"Unknown apply type: {context.Arguments.ApplyType}");
            return Task.CompletedTask;
        }

        // Use a unique type ID derived from the apply type so stacking is per-type
        var affectType = (uint)(0xFF00 | (int)applyType);
        player.AddAffect(affectType, applyType, context.Arguments.Value, 0,
            context.Arguments.Duration, 0);

        context.Player.SendChatMessage(
            $"Added affect {applyType} +{context.Arguments.Value} for {context.Arguments.Duration}s on {player.Name}");

        return Task.CompletedTask;
    }
}

public class AffectAddCommandOptions
{
    [Value(0, Required = true, HelpText = "EApplyType name (e.g. CRITICAL_PCT, MOV_SPEED)")]
    public string ApplyType { get; set; } = "";

    [Value(1, Required = true, HelpText = "Bonus value")]
    public int Value { get; set; }

    [Value(2, Default = 60, HelpText = "Duration in seconds (default 60)")]
    public int Duration { get; set; } = 60;

    [Value(3, HelpText = "Target player name (default self)")]
    public string? Target { get; set; }
}
