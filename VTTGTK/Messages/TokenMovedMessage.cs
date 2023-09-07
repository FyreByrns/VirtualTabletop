using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTTGTK.Messages;

namespace VTTGTK.Messages;
class TokenMovedMessage : Message, IMessageWithBody {
	public int FromX, FromY, ToX, ToY;

	public static Message ParseBodyData(byte[] buffer, int length) {
		int readIndex = 0;

		int fromX = Reader.Int32(buffer, ref readIndex);
		int fromY = Reader.Int32(buffer, ref readIndex);
		int toX = Reader.Int32(buffer, ref readIndex);
		int toY = Reader.Int32(buffer, ref readIndex);

		return new TokenMovedMessage(fromX, fromY, toX, toY);
	}

	protected override int GetBodyLength() {
		return sizeof(int) * 4;
	}

	protected override void CreateBody(List<byte> msgInProgress) {
		msgInProgress.AddRange(BitConverter.GetBytes(FromX));
		msgInProgress.AddRange(BitConverter.GetBytes(FromY));
		msgInProgress.AddRange(BitConverter.GetBytes(ToX));
		msgInProgress.AddRange(BitConverter.GetBytes(ToY));
	}

	public TokenMovedMessage(int fromX, int fromY, int toX, int toY) : base(MessageType.TokenMove) {
		FromX = fromX;
		FromY = fromY;
		ToX = toX;
		ToY = toY;
	}
}
