global using MessageLength_T = System.Int32;
using System.Reflection;
using System.Text;

namespace VTTGTK.Messages;

abstract class Message {
    public MessageType Type;

    protected virtual void CreateHeader(List<byte> msgInProgress) {
        msgInProgress.Add((byte)Type);
    }
    protected abstract MessageLength_T GetBodyLength();
    protected abstract void CreateBody(List<byte> msgInProgress);
    public byte[] GetBytes() {
        List<byte> bytes = new();
        CreateHeader(bytes);
        bytes.AddRange(BitConverter.GetBytes(GetBodyLength()));
        CreateBody(bytes);

        return bytes.ToArray();
    }

    public static MessageType[] ZeroLengthTypes = {
        MessageType.Connecting,
        MessageType.Disconnecting,
    };

    public static Dictionary<MessageType, Func<byte[], int, Message>> BodyDataParsers = new();

    protected Message(MessageType type) {
        Type = type;
    }

    public static Message Parse(byte[] buffer, int length) {
        if (length > buffer.Length) {
            Console.WriteLine(
                $"received ({buffer.Length}) is shorter than required ({length})");
            return null;
        }
        if (length == 0 || buffer.Length == 0) {
            Console.WriteLine("no data!");
            return null;
        }

        int readIndex = 0;
        byte byteType = Reader.Byte(buffer, ref readIndex);
        MessageType type = (MessageType)byteType;

        // if the message type never has a body, then parsing is super simple
        if (ZeroLengthTypes.Contains(type)) {
            return new BlankMessage(type);
        }

        // otherwise, need to figure out length
        MessageLength_T messageLength = 0;
        if (typeof(MessageLength_T) == typeof(int)) {
            messageLength = Reader.Int32(buffer, ref readIndex);
        }

        MessageLength_T actualLength = buffer.Length - readIndex;
        if (messageLength > actualLength) {
            Console.WriteLine($"malformed message:" +
                $"\n\twrong data size:" +
                $"\n\t\tspecified: {messageLength}" +
                $"\n\t\tactual   : {actualLength}");

            Console.WriteLine(
                $"message ({type}): " +
                $"{Encoding.ASCII.GetString(buffer, 0, length)}");

            foreach (byte b in buffer) {
                Console.Write($"{b} ");
            }
            Console.WriteLine();

            return null;
        }

        byte[] bodyData = buffer[readIndex..(readIndex + messageLength)];

        if (BodyDataParsers.ContainsKey(type)) {
            return BodyDataParsers[type](bodyData, messageLength);
        }

        return null;
    }

    static Message() {
        // grab messagetype attributes from message classes
        // .. and associate ParseBodyData methods
        foreach(var msgType in Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsAssignableTo(typeof(Message)))) {
            foreach(var type in msgType.GetCustomAttributes<MessageTypeAttribute>()) {
                MethodInfo info = msgType.GetMethod("ParseBodyData");
                
                var parseBodyData = delegate (byte[] bodyData, int messageLength) {
                    return (Message)info!.Invoke(null, new object[] { bodyData, messageLength })!;
                };

                BodyDataParsers[type.Type] = parseBodyData;
            }
        }
    }
}
