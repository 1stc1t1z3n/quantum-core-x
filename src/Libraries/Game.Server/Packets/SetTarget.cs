using QuantumCore.Networking;

namespace QuantumCore.Game.Packets;

[Packet(0x3f, EDirection.OUTGOING)]
[PacketGenerator]
public partial class SetTarget
{
    [Field(0)] public uint TargetVid { get; set; }
    [Field(1)] public byte Percentage { get; set; }
    // ENABLE_VIEW_TARGET_DECIMAL_HP adds these two fields
    [Field(2)] public int MinHp { get; set; }
    [Field(3)] public int MaxHp { get; set; }
}
