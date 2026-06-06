using Microsoft.Extensions.Logging;
using QuantumCore.API;
using QuantumCore.API.Game.Types.Skills;
using QuantumCore.API.PluginTypes;
using QuantumCore.Game.Packets;

namespace QuantumCore.Game.PacketHandlers.Game;

public class UseSkillHandler : IGamePacketHandler<PlayerUseSkill>
{
    private readonly ILogger<UseSkillHandler> _logger;

    public UseSkillHandler(ILogger<UseSkillHandler> logger)
    {
        _logger = logger;
    }

    public Task ExecuteAsync(GamePacketContext<PlayerUseSkill> ctx, CancellationToken token = default)
    {
        var player = ctx.Connection.Player;
        if (player is null) return Task.CompletedTask;

        if (!Enum.IsDefined(typeof(ESkill), (byte)ctx.Packet.SkillId))
        {
            _logger.LogDebug("Unknown skill id {SkillId}", ctx.Packet.SkillId);
            return Task.CompletedTask;
        }

        var skillId = (ESkill)(byte)ctx.Packet.SkillId;
        player.Skills.UseActiveSkill(skillId);
        return Task.CompletedTask;
    }
}
