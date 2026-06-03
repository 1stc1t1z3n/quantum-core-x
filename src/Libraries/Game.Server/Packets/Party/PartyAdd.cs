using QuantumCore.Networking;

namespace QuantumCore.Game.Packets.Party;

[Packet(0x4E, EDirection.OUTGOING)]
[PacketGenerator]
public partial class PartyAdd
{
    [Field(0)] public uint Pid { get; set; }

    [Field(1, Length = 25)]
    public string Name { get; set; } = "";
}
