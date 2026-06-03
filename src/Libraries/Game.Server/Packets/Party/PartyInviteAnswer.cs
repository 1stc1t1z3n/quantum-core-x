using QuantumCore.Networking;

namespace QuantumCore.Game.Packets.Party;

[Packet(0x49, EDirection.INCOMING)]
[PacketGenerator]
public partial class PartyInviteAnswer
{
    [Field(0)] public uint LeaderPid { get; set; }
    [Field(1)] public byte Accept { get; set; }
}
