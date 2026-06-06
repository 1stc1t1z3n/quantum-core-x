using QuantumCore.API;
using QuantumCore.API.PluginTypes;
using QuantumCore.Game.Packets;

namespace QuantumCore.Game.PacketHandlers.Game;

public class AddFlyTargetingHandler : IGamePacketHandler<AddFlyTargeting>
{
    public Task ExecuteAsync(GamePacketContext<AddFlyTargeting> ctx, CancellationToken token = default)
        => Task.CompletedTask;
}
