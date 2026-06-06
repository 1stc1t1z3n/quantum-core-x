using QuantumCore.Networking;

namespace QuantumCore.Game.Packets;

[Packet(0xDC, EDirection.OUTGOING)]
[PacketGenerator]
public partial class AttackBroadcast
{
    [Field(0)] public uint  AttackerVid   { get; set; }
    [Field(1)] public uint  VictimVid     { get; set; }
    [Field(2)] public int   Packet        { get; set; }
    [Field(3)] public int   DestX         { get; set; }
    [Field(4)] public int   DestY         { get; set; }
    [Field(5)] public float SyncDestX     { get; set; }
    [Field(6)] public float SyncDestY     { get; set; }
    [Field(7)] public uint  BlendDuration { get; set; }
}
