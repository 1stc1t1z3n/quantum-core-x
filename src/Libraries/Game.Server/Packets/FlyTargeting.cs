using QuantumCore.Networking;

namespace QuantumCore.Game.Packets;

[Packet(0x33, EDirection.INCOMING, Sequence = true)]
[PacketGenerator]
public partial class FlyTargeting
{
    [Field(0)] public uint TargetVid { get; set; }
    [Field(1)] public int  X         { get; set; }
    [Field(2)] public int  Y         { get; set; }
}
