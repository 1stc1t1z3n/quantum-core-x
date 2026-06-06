using QuantumCore.API;
using QuantumCore.API.PluginTypes;
using QuantumCore.Game.Packets;

namespace QuantumCore.Game.PacketHandlers.Game;

public class TargetInfoLoadHandler : IGamePacketHandler<TargetInfoLoad>
{
    public Task ExecuteAsync(GamePacketContext<TargetInfoLoad> ctx, CancellationToken token = default)
    {
        return Task.CompletedTask;
    }
}
