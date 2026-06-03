using QuantumCore.API;
using QuantumCore.API.PluginTypes;
using QuantumCore.Game.Extensions;
using QuantumCore.Game.Packets;

namespace QuantumCore.Game.PacketHandlers.Game;

public class WhisperHandler : IGamePacketHandler<WhisperIncoming>
{
    public Task ExecuteAsync(GamePacketContext<WhisperIncoming> ctx, CancellationToken token = default)
    {
        var sender = ctx.Connection.Player;
        if (sender is null) return Task.CompletedTask;

        var target = sender.Map!.World.GetPlayers()
            .FirstOrDefault(p => p.Name == ctx.Packet.NameTo);

        if (target is null)
        {
            sender.Connection.Send(new WhisperOutcoming
            {
                Type = 1, // WHISPER_TYPE_NOT_EXIST
                NameFrom = ctx.Packet.NameTo,
                Message = ""
            });
            return Task.CompletedTask;
        }

        target.Connection.Send(new WhisperOutcoming
        {
            Type = 0, // WHISPER_TYPE_CHAT
            NameFrom = sender.Name,
            Message = ctx.Packet.Message
        });

        return Task.CompletedTask;
    }
}
