using QuantumCore.Networking;

namespace QuantumCore.Game.Packets.Party;

// TPacketGCPartyUpdate: DWORD pid, BYTE state, BYTE percent_hp, short affects[7]
[Packet(0x4F, EDirection.OUTGOING)]
[PacketGenerator]
public partial class PartyUpdate
{
    [Field(0)] public uint Pid { get; set; }
    [Field(1)] public byte State { get; set; }
    [Field(2)] public byte PercentHp { get; set; }
    [Field(3, ArrayLength = 7)] public short[] Affects { get; set; } = new short[7];
}
