namespace VTTGTK.Messages;

[MessageType(MessageType.LobbyState)]
class LobbyStateMessage : Message, IMessageWithBody {
    public LobbyState LobbyState;

    public static Message ParseBodyData(byte[] buffer, int length) {
        LobbyState state = new();

        return new LobbyStateMessage(state);
    }

    protected override int GetBodyLength() {
        return 0;
    }
    protected override void CreateBody(List<byte> msgInProgress) {
    }

    public LobbyStateMessage(LobbyState state) : base(MessageType.LobbyState) {
        LobbyState = state;
    }
}
