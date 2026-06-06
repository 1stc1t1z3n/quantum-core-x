using QuantumCore.API;
using QuantumCore.API.PluginTypes;
using QuantumCore.Game.Packets;

namespace QuantumCore.Game.PacketHandlers.Game;

public class FlyTargetingHandler : IGamePacketHandler<FlyTargeting>
{
    public Task ExecuteAsync(GamePacketContext<FlyTargeting> ctx, CancellationToken token = default)
        => Task.CompletedTask;
}
