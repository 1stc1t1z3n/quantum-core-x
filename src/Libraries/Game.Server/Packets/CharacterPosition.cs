using QuantumCore.Networking;

namespace QuantumCore.Game.Packets;

// Client sends when player sits or stands (HEADER_CG_CHARACTER_POSITION = 0x1C).
[Packet(0x1C, EDirection.INCOMING, Sequence = true)]
[PacketGenerator]
public partial class CharacterPosition
{
    [Field(0)] public byte Position { get; set; }
}
