using CommandLine;
using QuantumCore.API.Game;
using QuantumCore.API.Game.World;
using QuantumCore.Game.Packets;

namespace QuantumCore.Game.Commands;

[Command("motion", "Play a motion animation on your character")]
public class MotionCommand : ICommandHandler<MotionOptions>
{
    public Task ExecuteAsync(CommandContext<MotionOptions> ctx)
    {
        var player = ctx.Player;
        var packet = new Motion
        {
            Vid = player.Vid,
            VictimVid = 0,
            MotionId = ctx.Arguments.MotionId
        };

        // Send to self and all nearby players
        player.Connection.Send(packet);
        foreach (var entity in player.NearbyEntities)
        {
            if (entity is IPlayerEntity nearby)
                nearby.Connection.Send(packet);
        }

        return Task.CompletedTask;
    }
}

public class MotionOptions
{
    [Value(0, MetaName = "id", Required = true)]
    public ushort MotionId { get; set; }
}
