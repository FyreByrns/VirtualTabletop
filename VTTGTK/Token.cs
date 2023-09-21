using FSerialization;

namespace VTTGTK;

class Token {
	[SerializeAndDeserialize(0, 0, 4)]
	public int X;
	[SerializeAndDeserialize(1, 4, 8)]
	public int Y;
	[SerializeAndDeserialize(2, 8, 12)]
	public int Size = 1;

	public const int TotalFieldSize = sizeof(int) * 3;
}


class TokenSerializerDeserializer : SerializerDeserializer<Token> {
	class Ser : Serializer<Token> {
		public override byte[] Serialize(Token value) {
			throw new NotImplementedException();
		}
	}
	class Der : Deserializer<Token> {
		public override Token Deserialize(byte[] bytes) {
			throw new NotImplementedException();
		}
	}

	public TokenSerializerDeserializer() 
		: base(new Ser(), new Der()) {
	}
}