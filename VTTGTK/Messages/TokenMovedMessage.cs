using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTTGTK.Messages;

using static FSerialization.FSerializationLogic;

namespace VTTGTK.Messages;

[MessageType(MessageType.TokenMove)]
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

[MessageType(MessageType.TokenCreate)]
class TokenCreateMessage : Message, IMessageWithBody {
	public Token Token;

	public static Message ParseBodyData(byte[] buffer, int length) {
		int readIndex = 0;

		Token? result = new();
		if (TryDeserialize(buffer, ref result)) {
			return new TokenCreateMessage(result!);
		}

		return default;
	}

	protected override void CreateBody(List<byte> msgInProgress) {
		TrySerialize(Token, out byte[] buf);
		msgInProgress.AddRange(buf);
	}

	protected override int GetBodyLength() {
		return Token.TotalFieldSize;
	}

	public TokenCreateMessage(Token token) : base(MessageType.TokenCreate) {
		Token = token;
	}
}