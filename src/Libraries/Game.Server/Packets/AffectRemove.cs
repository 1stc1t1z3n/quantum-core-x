using QuantumCore.Networking;

namespace QuantumCore.Game.Packets;

// TPacketGCAffectRemove: BYTE header, DWORD dwType, BYTE bApplyOn
[Packet(0x7F, EDirection.OUTGOING)]
[PacketGenerator]
public partial class AffectRemove
{
    [Field(0)] public uint Type { get; set; }
    [Field(1)] public byte ApplyOn { get; set; }
}
