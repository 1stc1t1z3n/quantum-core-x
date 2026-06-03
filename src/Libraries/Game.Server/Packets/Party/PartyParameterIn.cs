using QuantumCore.Networking;

namespace QuantumCore.Game.Packets.Party;

[Packet(0x4E, EDirection.INCOMING)]
[PacketGenerator]
public partial class PartyParameterIn
{
    [Field(0)] public byte DistributeMode { get; set; }
}
