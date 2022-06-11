using System.Collections.Generic;

public class PlayerLevelData : WordCuteDatabase
{
	public static string dataBaseName = "PlayeLevelData";

	public int level;

	public bool isFinish;

	public bool isPass;

	public int useCoinTimes;

	public int starCount;

	public List<string> selectWords;

	public List<int> specilIndexs;

	public PlayerLevelData()
	{
	}

	public PlayerLevelData(int level)
	{
		this.level = level;
		selectWords = new List<string>();
		specilIndexs = new List<int>();
	}

	public override void ReadLine(ByteBuffer buffer)
	{
		base.ReadLine(buffer);
		level = buffer.ReadShort();
		isFinish = ((buffer.ReadByte() == 1) ? true : false);
		isPass = ((buffer.ReadByte() == 1) ? true : false);
		useCoinTimes = buffer.ReadShort();
		starCount = buffer.ReadByte();
		ReadSelectWords(buffer);
		ReadSpecilIndex(buffer);
	}

	private void ReadSelectWords(ByteBuffer buffer)
	{
		int num = buffer.ReadByte();
		selectWords = new List<string>();
		for (int i = 0; i < num; i++)
		{
			selectWords.Add(buffer.ReadString());
		}
	}

	private void ReadSpecilIndex(ByteBuffer buffer)
	{
		int num = buffer.ReadByte();
		specilIndexs = new List<int>();
		for (int i = 0; i < num; i++)
		{
			specilIndexs.Add(buffer.ReadByte());
		}
	}

	public override void WriteLine(ByteBuffer buffer)
	{
		base.WriteLine(buffer);
		buffer.WriteShort((ushort)level);
		buffer.WriteByte((byte)(isFinish ? 1u : 0u));
		buffer.WriteByte((byte)(isPass ? 1u : 0u));
		buffer.WriteShort((ushort)useCoinTimes);
		buffer.WriteByte((byte)starCount);
		WriteSelectWords(buffer);
		WriteSpecilIndex(buffer);
	}

	private void WriteSelectWords(ByteBuffer buffer)
	{
		int num = ((!selectWords.IsEmpty()) ? selectWords.Count : 0);
		buffer.WriteByte((byte)num);
		for (int i = 0; i < num; i++)
		{
			buffer.WriteString(selectWords[i]);
		}
	}

	private void WriteSpecilIndex(ByteBuffer buffer)
	{
		int num = ((!specilIndexs.IsEmpty()) ? specilIndexs.Count : 0);
		buffer.WriteByte((byte)num);
		for (int i = 0; i < num; i++)
		{
			buffer.WriteByte((byte)specilIndexs[i]);
		}
	}

	public void AddSelectWord(string word, bool isSave = true)
	{
		selectWords.Add(word);
		if (isSave)
		{
			Save(level);
		}
	}

	public void AddSpecilIndex(int index, bool isSave = true)
	{
		specilIndexs.Add(index);
		if (isSave)
		{
			Save(level);
		}
	}

	public void RemoveSpecilIndex(int index, bool isSave = true)
	{
		specilIndexs.Remove(index);
		if (isSave)
		{
			Save(level);
		}
	}

	public void AddUseCoinTimes(bool isSave = true)
	{
		useCoinTimes++;
		if (isSave)
		{
			Save(level);
		}
	}

	public void SetFinish()
	{
		isFinish = true;
		isPass = true;
		Save(level);
	}

	public void ClearnSelect()
	{
		isFinish = false;
		useCoinTimes = 0;
		selectWords.Clear();
		specilIndexs.Clear();
		Save(level);
	}

	public static void Save(int level)
	{
		SingleObject<GameDataBaseManger>.instance.SaveData<PlayerLevelData>(dataBaseName, string.Empty, level);
	}

	public static PlayerLevelData GetByLevel(int level)
	{
		List<PlayerLevelData> list = SingleObject<GameDataBaseManger>.instance.ReadData(dataBaseName, (PlayerLevelData line) => line.level == level, string.Empty, level);
		if (list.IsEmpty())
		{
			PlayerLevelData playerLevelData = new PlayerLevelData(level);
			SingleObject<GameDataBaseManger>.instance.InsertData(dataBaseName, playerLevelData, string.Empty, level);
			Save(level);
			return playerLevelData;
		}
		return list[0];
	}
}
