using QuantumCore.Networking;

namespace QuantumCore.Game.Packets.Party;

[Packet(0x53, EDirection.OUTGOING)]
[PacketGenerator]
public partial class PartyParameterOut
{
    [Field(0)] public byte DistributeMode { get; set; }
}
