using QuantumCore.Networking;

namespace QuantumCore.Game.Packets;

[Packet(0xFE, EDirection.INCOMING)]
[PacketGenerator]
public partial class Pong
{
}
