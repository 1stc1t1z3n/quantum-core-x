using QuantumCore.Networking;

namespace QuantumCore.Game.Packets;

// Client sends HEADER_CG_HACK (0x69) for anti-hack check. Payload: char szBuf[256].
[Packet(0x69, EDirection.INCOMING, Sequence = true)]
[PacketGenerator]
public partial class HackCheck
{
    [Field(0, Length = 255)] public string Message { get; set; } = "";
}
