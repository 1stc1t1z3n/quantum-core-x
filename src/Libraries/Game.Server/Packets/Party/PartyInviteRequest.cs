using QuantumCore.Networking;

namespace QuantumCore.Game.Packets.Party;

// Client sends VID of the target player to invite
[Packet(0x48, EDirection.INCOMING)]
[PacketGenerator]
public partial class PartyInviteRequest
{
    [Field(0)] public uint Vid { get; set; }
}
