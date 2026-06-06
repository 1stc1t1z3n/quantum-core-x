using QuantumCore.Networking;

namespace QuantumCore.Game.Packets;

[Packet(0x3B, EDirection.INCOMING, Sequence = true)]
[PacketGenerator]
public partial class TargetInfoLoad
{
    [Field(0)] public uint Vid { get; set; }
}
