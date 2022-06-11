using System;

public class WordCuteDatabase : IGameDataBase
{
	public int index;

	public DateTime time = DateTime.Now;

	public virtual void ReadLine(ByteBuffer buffer)
	{
		time = new DateTime(long.Parse(buffer.ReadString()));
	}

	public virtual void WriteLine(ByteBuffer buffer)
	{
		buffer.WriteString(time.Ticks.ToString());
	}

	public static void DeleteData<T>(string databaseName, int aleryDay) where T : WordCuteDatabase, new()
	{
		DateTime now = DateTime.Now;
	}
}
