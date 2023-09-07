namespace VTTGTK.Messages;

interface IMessageWithBody
{
    static abstract Message ParseBodyData(byte[] buffer, int length);
}
