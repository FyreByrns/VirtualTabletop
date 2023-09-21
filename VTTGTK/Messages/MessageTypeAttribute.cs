namespace VTTGTK.Messages;

public class MessageTypeAttribute : Attribute {
    public MessageType Type;

    public MessageTypeAttribute(MessageType type) {
		Type = type;
	}
}
