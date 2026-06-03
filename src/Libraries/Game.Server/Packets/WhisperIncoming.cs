using QuantumCore.Networking;

namespace QuantumCore.Game.Packets;

// TPacketCGWhisper: WORD wSize, char szNameTo[26], + message body
[Packet(0x13, EDirection.INCOMING, Sequence = true)]
[PacketGenerator]
public partial class WhisperIncoming
{
    [Field(0)] public ushort Size => (ushort)Message.Length;

    [Field(1, Length = 25)]
    public string NameTo { get; set; } = "";

    public string Message { get; set; } = "";
}
