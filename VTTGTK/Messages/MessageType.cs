namespace VTTGT.Messages;

public enum MessageType : byte
{
    None = 0,

    IDMessage,

    Connecting,
    Disconnecting,

    InfoRequest,
    InfoResponse,

    LobbyState,
}
