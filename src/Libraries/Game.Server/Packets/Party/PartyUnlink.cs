using QuantumCore.Networking;

namespace QuantumCore.Game.Packets.Party;

[Packet(0x5C, EDirection.OUTGOING)]
[PacketGenerator]
public partial class PartyUnlink
{
    [Field(0)] public uint Pid { get; set; }
    [Field(1)] public uint Vid { get; set; }
}
