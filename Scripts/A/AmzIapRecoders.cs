using System.Collections.Generic;

internal class AmzIapRecoders : IGameDataBase
{
	public static string dataBaseName = "AmzIapRecoders";

	private static int lineIndex;

	public string receiptId = string.Empty;

	public string userId = string.Empty;

	public string goodId = string.Empty;

	public int state;

	public AmzIapRecoders()
	{
	}

	public AmzIapRecoders(string receiptId, string userId, string goodId, int state)
	{
		this.receiptId = receiptId;
		this.userId = userId;
		this.goodId = goodId;
		this.state = state;
	}

	public void ReadLine(ByteBuffer buffer)
	{
		receiptId = buffer.ReadString();
		userId = buffer.ReadString();
		goodId = buffer.ReadString();
		state = buffer.ReadByte();
	}

	public void WriteLine(ByteBuffer buffer)
	{
		buffer.WriteString(receiptId);
		buffer.WriteString(userId);
		buffer.WriteString(goodId);
		buffer.WriteByte((byte)state);
	}

	public static void InertLine(string receiptId, string userId, string goodId, int state)
	{
		AmzIapRecoders line = new AmzIapRecoders(receiptId, userId, goodId, state);
		SingleObject<GameDataBaseManger>.instance.InsertData(dataBaseName, line, string.Empty);
	}

	public static AmzIapRecoders GetLineByReceiptId(string receiptId)
	{
		List<AmzIapRecoders> list = SingleObject<GameDataBaseManger>.instance.ReadData(dataBaseName, (AmzIapRecoders line) => line.receiptId == receiptId, string.Empty);
		if (list == null || list.Count == 0)
		{
			return null;
		}
		return list[0];
	}

	public static void RemoveLineByReceiptId(string receiptId)
	{
		SingleObject<GameDataBaseManger>.instance.DeleteData(dataBaseName, (AmzIapRecoders line) => line.receiptId == receiptId, string.Empty);
	}
}
