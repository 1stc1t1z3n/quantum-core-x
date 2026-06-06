using QuantumCore.API;
using QuantumCore.API.PluginTypes;
using QuantumCore.Game.Packets;

namespace QuantumCore.Game.PacketHandlers.Game;

public class HackCheckHandler : IGamePacketHandler<HackCheck>
{
    public Task ExecuteAsync(GamePacketContext<HackCheck> ctx, CancellationToken token = default)
        => Task.CompletedTask;
}
