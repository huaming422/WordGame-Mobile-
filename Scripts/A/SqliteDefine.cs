using UnityEngine;

public class SqliteDefine
{
	public static string dbPath = "AllWordData/";

	public static string dbName = "words.db";

	public static string tableName = "words";

	public static string connectString
	{
		get
		{
			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				return "data source =" + Util.streamingAssetsPath + "AllWordData/words.db";
			}
			return "data source =" + Util.persistentDataPath + "AllWordData/words.db";
		}
	}
}
