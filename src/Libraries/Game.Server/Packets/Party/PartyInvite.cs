using QuantumCore.Networking;

namespace QuantumCore.Game.Packets.Party;

[Packet(0x4D, EDirection.OUTGOING)]
[PacketGenerator]
public partial class PartyInvite
{
    [Field(0)] public uint LeaderPid { get; set; }
}
