namespace VTTGTK.Messages;

class InfoRequestMessage : Message, IMessageWithBody
{
    public InfoType RequestType;

    public static Message ParseBodyData(byte[] buffer, int length)
    {
        int readIndex = 0;

        byte requestTypeByte = Reader.Byte(buffer, ref readIndex);
        InfoType requestType = (InfoType)requestTypeByte;

        return new InfoRequestMessage(requestType);
    }

    protected override MessageLength_T GetBodyLength()
    {
        return sizeof(InfoType);
    }
    protected override void CreateBody(List<byte> msgInProgress)
    {
        msgInProgress.Add((byte)RequestType);
    }

    public InfoRequestMessage(InfoType requestType) : base(MessageType.InfoRequest)
    {
        RequestType = requestType;
    }
}
