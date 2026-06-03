namespace QuantumCore.API.Game.Types;

public enum ChatMessageType : byte
{
    NORMAL = 0,
    INFO = 1,
    NOTICE = 2,
    GROUP = 3,  // party chat
    GUILD = 4,
    COMMAND = 5,
    SHOUT = 6,
    // 7 = WHISPER (client-only, not sent by server)
    BIG_NOTICE = 8,
}
