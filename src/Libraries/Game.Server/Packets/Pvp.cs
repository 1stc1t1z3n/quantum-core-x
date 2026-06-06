using QuantumCore.Networking;

namespace QuantumCore.Game.Packets;

[Packet(0x29, EDirection.OUTGOING)]
[PacketGenerator]
public partial class Pvp
{
    [Field(0)] public uint VidSrc { get; set; }
    [Field(1)] public uint VidDst { get; set; }
    [Field(2)] public byte Mode  { get; set; }
}
