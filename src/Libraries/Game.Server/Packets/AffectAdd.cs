using QuantumCore.Networking;

namespace QuantumCore.Game.Packets;

// TPacketGCAffectAdd: BYTE header + TPacketAffectElement
// TPacketAffectElement: DWORD dwType, BYTE bPointIdxApplyOn, long lApplyValue, DWORD dwFlag, long lDuration, long lSPCost
[Packet(0x7E, EDirection.OUTGOING)]
[PacketGenerator]
public partial class AffectAdd
{
    [Field(0)] public uint Type { get; set; }
    [Field(1)] public byte PointApplyOn { get; set; }
    [Field(2)] public int ApplyValue { get; set; }
    [Field(3)] public uint Flag { get; set; }
    [Field(4)] public int Duration { get; set; }
    [Field(5)] public int SpCost { get; set; }
}
