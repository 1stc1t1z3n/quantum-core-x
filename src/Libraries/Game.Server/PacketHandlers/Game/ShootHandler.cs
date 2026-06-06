using QuantumCore.API;
using QuantumCore.API.PluginTypes;
using QuantumCore.Game.Packets;

namespace QuantumCore.Game.PacketHandlers.Game;

public class ShootHandler : IGamePacketHandler<Shoot>
{
    public Task ExecuteAsync(GamePacketContext<Shoot> ctx, CancellationToken token = default)
        => Task.CompletedTask;
}
