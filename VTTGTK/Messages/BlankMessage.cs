namespace VTTGT.Messages;

class BlankMessage : Message
{
    protected override int GetBodyLength()
    {
        return 0;
    }
    protected override void CreateBody(List<byte> msgInProgress) { }
    public BlankMessage(MessageType type) : base(type) { }
}
