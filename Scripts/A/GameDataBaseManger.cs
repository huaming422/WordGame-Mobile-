using System;
using System.Collections.Generic;
using System.IO;

public class GameDataBaseManger : SingleObject<GameDataBaseManger>
{
	private string nowUserId = "guest";

	private bool isInited;

	private Dictionary<string, object> _allDataBaseCahe = new Dictionary<string, object>();

	public static string dataBaseDir
	{
		get
		{
			return AppConst.DataDir + "DataBase/";
		}
	}

	public string dataBaseFileDir
	{
		get
		{
			return dataBaseDir + nowUserId + "/";
		}
	}

	public void Init(string userID = "guest")
	{
		if (!isInited)
		{
			nowUserId = userID;
			if (!Directory.Exists(dataBaseFileDir))
			{
				Directory.CreateDirectory(dataBaseFileDir);
			}
			isInited = true;
		}
	}

	public void Clearn()
	{
		if (Directory.Exists(dataBaseDir))
		{
			Directory.Delete(dataBaseDir, true);
		}
		Directory.CreateDirectory(dataBaseFileDir);
	}

	public void CleranDB(string databaeName, string type = "", int typeIndex = 1)
	{
		if (isInited)
		{
			string path = dataBaseFileDir + databaeName + type + typeIndex;
			if (File.Exists(path))
			{
				File.Delete(path);
			}
		}
	}

	public List<T> ReadData<T>(string databaeName, string type = "", int typeIndex = 1) where T : IGameDataBase, new()
	{
		if (!isInited)
		{
			return null;
		}
		string text = dataBaseFileDir + databaeName + type + typeIndex;
		if (_allDataBaseCahe.ContainsKey(text))
		{
			return _allDataBaseCahe[text] as List<T>;
		}
		if (!File.Exists(text))
		{
			return null;
		}
		byte[] data = File.ReadAllBytes(text);
		ByteBuffer byteBuffer = new ByteBuffer(data);
		List<T> list = new List<T>();
		while (byteBuffer.readPos < byteBuffer.dataLength)
		{
			T item = new T();
			item.ReadLine(byteBuffer);
			list.Add(item);
		}
		_allDataBaseCahe.Add(text, list);
		return list;
	}

	public List<T> ReadData<T>(string databaeName, Func<T, bool> func, string type = "", int typeIndex = 1) where T : IGameDataBase, new()
	{
		if (!isInited)
		{
			return null;
		}
		List<T> list = ReadData<T>(databaeName, type, typeIndex);
		if (list.IsEmpty() || func == null)
		{
			return list;
		}
		List<T> list2 = new List<T>();
		for (int i = 0; i < list.Count; i++)
		{
			if (func(list[i]))
			{
				list2.Add(list[i]);
			}
		}
		return list2;
	}

	public void SaveData<T>(string databaeName, string type = "", int typeIndex = 1, List<T> dataBase = null) where T : IGameDataBase
	{
		if (!isInited)
		{
			return;
		}
		string text = dataBaseFileDir + databaeName + type + typeIndex;
		if (dataBase == null && _allDataBaseCahe.ContainsKey(text))
		{
			dataBase = _allDataBaseCahe[text] as List<T>;
		}
		ByteBuffer byteBuffer = new ByteBuffer();
		if (!dataBase.IsEmpty())
		{
			for (int i = 0; i < dataBase.Count; i++)
			{
				dataBase[i].WriteLine(byteBuffer);
			}
		}
		if (!Directory.Exists(dataBaseFileDir))
		{
			Directory.CreateDirectory(dataBaseFileDir);
		}
		File.WriteAllBytes(text, byteBuffer.ToBytes());
	}

	public void InsertData<T>(string databaeName, T line, string type = "", int typeIndex = 1, bool isSave = true) where T : IGameDataBase, new()
	{
		if (isInited && line != null)
		{
			List<T> list = ReadData<T>(databaeName, type, typeIndex);
			if (list == null)
			{
				list = new List<T>();
			}
			list.Add(line);
			string text = dataBaseFileDir + databaeName + type + typeIndex;
			if (!_allDataBaseCahe.ContainsKey(text))
			{
				_allDataBaseCahe.Add(text, list);
			}
			if (!Directory.Exists(dataBaseFileDir))
			{
				Directory.CreateDirectory(dataBaseFileDir);
			}
			if (isSave)
			{
				FileStream fileStream = File.Open(text, FileMode.OpenOrCreate, FileAccess.ReadWrite);
				fileStream.Seek(0L, SeekOrigin.End);
				ByteBuffer byteBuffer = new ByteBuffer();
				line.WriteLine(byteBuffer);
				byte[] array = byteBuffer.ToBytes();
				fileStream.Write(array, 0, array.Length);
				fileStream.Flush();
				fileStream.Close();
			}
		}
	}

	public void DeleteData<T>(string databaeName, Func<T, bool> needClearn = null, string type = "", int typeIndex = 1) where T : IGameDataBase, new()
	{
		if (!isInited)
		{
			return;
		}
		List<T> list = ReadData<T>(databaeName, type, typeIndex);
		if (list.IsEmpty())
		{
			return;
		}
		if (needClearn == null)
		{
			list.Clear();
		}
		else
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (needClearn(list[i]))
				{
					list.RemoveAt(i);
					i--;
				}
			}
		}
		SaveData<T>(databaeName, type, typeIndex);
	}

	public void Close()
	{
		isInited = false;
		nowUserId = null;
		_allDataBaseCahe.Clear();
	}
}
