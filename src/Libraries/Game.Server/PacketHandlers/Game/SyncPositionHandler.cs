using QuantumCore.API;
using QuantumCore.API.PluginTypes;
using QuantumCore.Game.Packets;

namespace QuantumCore.Game.PacketHandlers.Game;

public class SyncPositionHandler : IGamePacketHandler<SyncPosition>
{
    public Task ExecuteAsync(GamePacketContext<SyncPosition> ctx, CancellationToken token = default)
        => Task.CompletedTask;
}
