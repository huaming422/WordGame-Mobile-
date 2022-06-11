using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WorldDesignUICtrl : MonoBehaviour
{
	public static int nowOpenLevel = 1;

	public static LevelData nowLevelData = null;

	private static Dictionary<string, List<int>> _usedWord = new Dictionary<string, List<int>>();

	public static string levelDataDir
	{
		get
		{
			return AppConst.TempDir + "LevelData/";
		}
	}

	private void Start()
	{
		if (Application.platform != RuntimePlatform.WindowsEditor)
		{
			string text = Util.persistentDataPath + SqliteDefine.dbPath;
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			string sourceFileName = Util.streamingAssetsPath + SqliteDefine.dbPath + SqliteDefine.dbName;
			string destFileName = text + SqliteDefine.dbName;
			File.Copy(sourceFileName, destFileName, true);
		}
		Debug.Log(Application.platform);
		SingleObject<DisignDBManager>.instance.Init();
		InitUseWord();
	}

	private static void InitUseWord()
	{
		if (!Directory.Exists(levelDataDir))
		{
			Directory.CreateDirectory(levelDataDir);
		}
		string[] files = Directory.GetFiles(levelDataDir);
		if (files.IsEmpty())
		{
			return;
		}
		for (int i = 0; i < files.Length; i++)
		{
			if (!files[i].EndsWith(".txt"))
			{
				continue;
			}
			string json = File.ReadAllText(files[i]);
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(files[i]);
			LevelData levelData = LevelData.ToLevelData(json);
			Word[] words = levelData.words;
			if (words.IsEmpty())
			{
				continue;
			}
			int item = int.Parse(fileNameWithoutExtension);
			for (int j = 0; j < words.Length; j++)
			{
				List<int> list = null;
				if (_usedWord.ContainsKey(words[j].word))
				{
					list = _usedWord[words[j].word];
				}
				else
				{
					list = new List<int>();
					_usedWord.Add(words[j].word, list);
				}
				list.Add(item);
			}
		}
	}

	public static void UpdateLeveData()
	{
		AlpHabetPos size = DisignGridCtrl.instance.GetSize();
		Word[] useWords = DisignGridCtrl.instance.GetUseWords();
		nowLevelData = new LevelData(size.h, size.v, useWords);
		string[] useAlphabets = DisignGridCtrl.instance.GetUseAlphabets(useWords);
		string text = string.Empty;
		for (int i = 0; i < useAlphabets.Length; i++)
		{
			text += useAlphabets[i];
		}
		nowLevelData.useAlphabets = text;
	}

	public static void UpdateInfo()
	{
		if (nowLevelData != null)
		{
			DisignInfoCtrl.instance.ShowUseAlphabet(nowLevelData.useAlphabets);
			DisignInfoCtrl.instance.ShowUseWords(nowLevelData.words);
			DisignInfoCtrl.instance.ShowUseGridSize(nowLevelData.gridHoriCount, nowLevelData.gridVertCount);
		}
	}

	public static void SaveLevelData(int level)
	{
		if (nowLevelData == null)
		{
			return;
		}
		List<string> list = CheckNowLevelhaveRepeat();
		if (!list.IsEmpty())
		{
			LogNowLevelUse(list);
			return;
		}
		string contents = nowLevelData.ToJson();
		if (!Directory.Exists(levelDataDir))
		{
			Directory.CreateDirectory(levelDataDir);
		}
		string path = levelDataDir + level + ".txt";
		File.WriteAllText(path, contents);
		nowOpenLevel = level;
		string arg = CheckHaveRepeatWordAndSaveUse();
		DisignInfoCtrl.instance.Log(string.Format("保存关卡{0}成功,{1}", level, arg));
	}

	public static void ForceSave(int level)
	{
		if (nowLevelData != null)
		{
			string contents = nowLevelData.ToJson();
			if (!Directory.Exists(levelDataDir))
			{
				Directory.CreateDirectory(levelDataDir);
			}
			string path = levelDataDir + level + ".txt";
			File.WriteAllText(path, contents);
			DisignInfoCtrl.instance.Log(string.Format("保存关卡{0} 位置成功", level));
		}
	}

	private static List<string> CheckNowLevelhaveRepeat()
	{
		Word[] words = nowLevelData.words;
		List<string> list = new List<string>();
		for (int i = 0; i < words.Length; i++)
		{
			int num = Array.FindIndex(words, (Word n) => n.word == words[i].word);
			if (num != i)
			{
				list.Add(words[i].word);
			}
		}
		return list;
	}

	private static void LogNowLevelUse(List<string> repeat)
	{
		string text = repeat[0];
		for (int i = 1; i < repeat.Count; i++)
		{
			text = text + "," + repeat[i];
		}
		DisignInfoCtrl.instance.LogError("保存失败 ！！本关重复使用：" + text);
	}

	private static string CheckHaveRepeatWordAndSaveUse()
	{
		string text = string.Empty;
		if (nowLevelData == null)
		{
			return string.Empty;
		}
		Word[] words = nowLevelData.words;
		if (words.IsEmpty())
		{
			return string.Empty;
		}
		for (int i = 0; i < words.Length; i++)
		{
			if (_usedWord.ContainsKey(words[i].word))
			{
				List<int> list = _usedWord[words[i].word];
				if (list.IsEmpty())
				{
					continue;
				}
				string text2 = string.Empty;
				for (int j = 0; j < list.Count; j++)
				{
					if (list[j] != nowOpenLevel)
					{
						text2 = text2 + list[j] + ",";
					}
				}
				if (!string.IsNullOrEmpty(text2))
				{
					string text3 = text;
					text = text3 + words[i].word + ":" + text2 + " ";
				}
				if (list.IndexOf(nowOpenLevel) == -1)
				{
					list.Add(nowOpenLevel);
				}
			}
			else
			{
				List<int> list2 = new List<int>();
				list2.Add(nowOpenLevel);
				_usedWord.Add(words[i].word, list2);
			}
		}
		if (!string.IsNullOrEmpty(text))
		{
			text = "重复使用：" + text;
		}
		return text;
	}

	public static void OpenLevel(int level)
	{
		string path = levelDataDir + level + ".txt";
		if (!File.Exists(path))
		{
			DisignInfoCtrl.instance.Log(string.Format("关卡{0}不存在", level));
			return;
		}
		string json = File.ReadAllText(path);
		nowLevelData = LevelData.ToLevelData(json);
		DisignGridCtrl.instance.ShowLevelData(nowLevelData);
		UpdateInfo();
		nowOpenLevel = level;
		DisignInfoCtrl.instance.Log(string.Format("打开关卡{0}成功", level));
	}
}
