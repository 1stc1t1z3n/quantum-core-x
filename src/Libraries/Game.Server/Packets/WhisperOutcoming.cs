using QuantumCore.Networking;

namespace QuantumCore.Game.Packets;

// TPacketGCWhisper: WORD wSize, BYTE bType, char szNameFrom[26], + message body
// Types: 0=chat, 1=not_exist, 2=target_blocked, 3=sender_blocked, 4=error, 5=GM, 0xFF=system
[Packet(0x22, EDirection.OUTGOING)]
[PacketGenerator]
public partial class WhisperOutcoming
{
    [Field(0)] public ushort Size => (ushort)Message.Length;
    [Field(1)] public byte Type { get; set; }

    [Field(2, Length = 25)]
    public string NameFrom { get; set; } = "";

    public string Message { get; set; } = "";
}
