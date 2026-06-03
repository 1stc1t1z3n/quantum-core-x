using QuantumCore.Networking;

namespace QuantumCore.Game.Packets.Party;

[Packet(0x50, EDirection.OUTGOING)]
[PacketGenerator]
public partial class PartyRemove
{
    [Field(0)] public uint Pid { get; set; }
}
