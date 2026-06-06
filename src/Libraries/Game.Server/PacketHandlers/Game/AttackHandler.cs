using Microsoft.Extensions.Logging;
using QuantumCore.API;
using QuantumCore.API.Game.World;
using QuantumCore.API.PluginTypes;
using QuantumCore.Game.Packets;

namespace QuantumCore.Game.PacketHandlers.Game;

public class AttackHandler : IGamePacketHandler<Attack>
{
    private readonly ILogger<AttackHandler> _logger;

    public AttackHandler(ILogger<AttackHandler> logger)
    {
        _logger = logger;
    }

    public Task ExecuteAsync(GamePacketContext<Attack> ctx, CancellationToken token = default)
    {
        var attacker = ctx.Connection.Player;
        if (attacker is null)
        {
            _logger.LogWarning("Attack without having a player instance");
            ctx.Connection.Close();
            return Task.CompletedTask;
        }

        var entity = attacker.Map?.GetEntity(ctx.Packet.Vid);
        if (entity is null)
        {
            return Task.CompletedTask;
        }

        _logger.LogDebug("Attack from {Attacker} with type {SkillMotion} target {TargetId}", attacker.Name,
            ctx.Packet.SkillMotion, ctx.Packet.Vid);

        attacker.Attack(entity);

        // Broadcast attack sync data to nearby clients so they can update the
        // victim's push position in real time instead of waiting for SYNC_POSITION.
        var broadcast = new AttackBroadcast
        {
            AttackerVid   = attacker.Vid,
            VictimVid     = ctx.Packet.Vid,
            Packet        = ctx.Packet.Packet,
            DestX         = ctx.Packet.DestX,
            DestY         = ctx.Packet.DestY,
            SyncDestX     = ctx.Packet.SyncDestX,
            SyncDestY     = ctx.Packet.SyncDestY,
            BlendDuration = ctx.Packet.BlendDuration,
        };

        foreach (var nearby in attacker.NearbyEntities)
        {
            if (nearby is IPlayerEntity player)
            {
                player.Connection.Send(broadcast);
            }
        }

        return Task.CompletedTask;
    }
}
