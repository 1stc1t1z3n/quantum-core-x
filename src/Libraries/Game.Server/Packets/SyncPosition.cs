using QuantumCore.Networking;

namespace QuantumCore.Game.Packets;

// Client sends SYNC_POSITION (0x08) after FlushVictimList (knockback push).
// wSize = 3 + N*12 (includes header byte per Metin2 convention).
// Followed by SendSequence() byte.
[Packet(0x08, EDirection.INCOMING, Sequence = true)]
[PacketGenerator]
public partial class SyncPosition
{
    // wSize in raw bytes = 3 + Data.Length; generator computes: __Size = wSize - 2 - 1 = Data.Length
    [Field(0)] public ushort Size => (ushort)(Data.Length + 3);
    public byte[] Data { get; set; } = [];
}
