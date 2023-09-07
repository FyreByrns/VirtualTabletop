namespace VTTGTK.Messages;

using ID_T = MessageLength_T;

class IDMessage : Message, IMessageWithBody
{
    public ID_T ID;

    public static Message ParseBodyData(byte[] buffer, int length)
    {
        int readIndex = 0;

        if (typeof(ID_T) == typeof(int))
        {
            IDMessage result = new(Reader.Int32(buffer, ref readIndex));
            return result;
        }

        return null;
    }

    protected override int GetBodyLength()
    {
        return sizeof(ID_T);
    }
    protected override void CreateBody(List<byte> msgInProgress)
    {
        msgInProgress.AddRange(BitConverter.GetBytes(ID));
    }

    public IDMessage(ID_T id) : base(MessageType.IDMessage)
    {
        ID = id;
    }
}
