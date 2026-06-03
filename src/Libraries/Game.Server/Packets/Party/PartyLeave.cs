using QuantumCore.Networking;

namespace QuantumCore.Game.Packets.Party;

// CG: leave party or kick a member (pid = target player id; 0 = self-leave)
[Packet(0x4A, EDirection.INCOMING)]
[PacketGenerator]
public partial class PartyLeave
{
    [Field(0)] public uint Pid { get; set; }
}
