using QuantumCore.Networking;

namespace QuantumCore.Game.Packets;

[Packet(0x36, EDirection.INCOMING, Sequence = true)]
[PacketGenerator]
public partial class Shoot
{
    [Field(0)] public byte Type { get; set; }
}
