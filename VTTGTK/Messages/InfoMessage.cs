using System.Text;

namespace VTTGT.Messages;

class InfoMessage : Message, IMessageWithBody {
    public InfoType RequestType;
    public string Contents;

    public static Message ParseBodyData(byte[] buffer, int length) {
        int readIndex = 0;

        byte requestTypeByte = Reader.Byte(buffer, ref readIndex);
        InfoType requestType = (InfoType)requestTypeByte;

        int responseLength = Reader.Int32(buffer, ref readIndex);
        string responseString = Reader.String(buffer, responseLength, ref readIndex);

        return new InfoMessage(requestType, responseString);
    }

    protected override int GetBodyLength() {
        return
            sizeof(InfoType) +   // type
            sizeof(int) +           // content length field
            Contents.Length;        // content length
    }
    protected override void CreateBody(List<byte> msgInProgress) {
        msgInProgress.Add((byte)RequestType);
        msgInProgress.AddRange(BitConverter.GetBytes(Contents.Length));
        msgInProgress.AddRange(Encoding.ASCII.GetBytes(Contents));
    }

    public InfoMessage(InfoType requestType, string contents)
        : base(MessageType.InfoResponse) {

        RequestType = requestType;
        Contents = contents;
    }
}