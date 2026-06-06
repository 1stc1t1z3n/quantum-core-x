using QuantumCore.API;
using QuantumCore.API.PluginTypes;
using QuantumCore.Game.Packets;

namespace QuantumCore.Game.PacketHandlers.Game;

public class CharacterPositionHandler : IGamePacketHandler<CharacterPosition>
{
    public Task ExecuteAsync(GamePacketContext<CharacterPosition> ctx, CancellationToken token = default)
        => Task.CompletedTask;
}
