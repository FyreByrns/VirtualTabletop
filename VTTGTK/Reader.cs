using System.Text;

namespace VTTGT;

static class Reader {
    public static byte Byte(byte[] buffer, ref int readIndex) {
        readIndex += sizeof(byte);
        return buffer[readIndex - sizeof(byte)];
    }
    public static int Int32(byte[] buffer, ref int readIndex) {
        readIndex += sizeof(int);
        return buffer[readIndex - sizeof(int)];
    }
    public static string String(byte[] buffer, int length, ref int readIndex) {
        return Encoding.ASCII.GetString(buffer, readIndex, length);
    }
}
