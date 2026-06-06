using QuantumCore.API.Game.Types.Skills;
using QuantumCore.Networking;

namespace QuantumCore.Game.Packets;

[Packet(0x02, EDirection.INCOMING, Sequence = true)]
[PacketGenerator]
public partial class Attack
{
    [Field(0)]  public ESkill SkillMotion   { get; set; }
    [Field(1)]  public uint   Vid           { get; set; }
    [Field(2)]  public int    Packet        { get; set; }
    [Field(3)]  public int    SrcX          { get; set; }
    [Field(4)]  public int    SrcY          { get; set; }
    [Field(5)]  public int    DestX         { get; set; }
    [Field(6)]  public int    DestY         { get; set; }
    [Field(7)]  public float  SyncDestX     { get; set; }
    [Field(8)]  public float  SyncDestY     { get; set; }
    [Field(9)]  public uint   BlendDuration { get; set; }
    [Field(10)] public uint   ComboMotion   { get; set; }
    [Field(11)] public uint   Time          { get; set; }
    [Field(12, ArrayLength = 2)] public byte[] Unknown { get; set; } = new byte[2] {0, 0};
}
