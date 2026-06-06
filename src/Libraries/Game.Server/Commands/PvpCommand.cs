using CommandLine;
using QuantumCore.API.Game;
using QuantumCore.API.Game.World;
using QuantumCore.Game.Packets;

namespace QuantumCore.Game.Commands;

[Command("pvp", "Mutually enable PVP fight mode between you and another player")]
[CommandNoPermission]
public class PvpCommand : ICommandHandler<PvpCommandOptions>
{
    private readonly IWorld _world;

    public PvpCommand(IWorld world)
    {
        _world = world;
    }

    public Task ExecuteAsync(CommandContext<PvpCommandOptions> context)
    {
        IPlayerEntity? target;
        if (uint.TryParse(context.Arguments.Target, out var vid))
            target = _world.GetPlayerById(vid);
        else
            target = _world.GetPlayer(context.Arguments.Target);

        if (target is null)
        {
            context.Player.SendChatInfo("Player not found");
            return Task.CompletedTask;
        }

        // PVP_MODE_FIGHT = 2 — both clients add each other to the PVP key map
        var packet = new Pvp
        {
            VidSrc = context.Player.Vid,
            VidDst = target.Vid,
            Mode = 2,
        };

        context.Player.Connection.Send(packet);
        target.Connection.Send(packet);

        context.Player.SendChatInfo($"PVP fight mode enabled with {target.Name}");

        return Task.CompletedTask;
    }
}

public class PvpCommandOptions
{
    [Value(0, Required = true)] public string Target { get; set; } = "";
}
