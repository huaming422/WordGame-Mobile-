using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordCuteTools
{
	public static LevelData ReadLevelData(int level)
	{
		GameAsset asset = new GameAsset(WordConstString.levelDataPath, level.ToString());
		TextAsset textAsset = SingleObject<ResourceManager>.instance.LoadAsset<TextAsset>(asset);
		if (textAsset == null)
		{
			return null;
		}
		return LevelData.ToLevelData(textAsset.text);
	}

	public static string[] StringToStingArray(string src)
	{
		if (src == null)
		{
			return null;
		}
		string[] array = new string[src.Length];
		for (int i = 0; i < src.Length; i++)
		{
			array[i] = src[i].ToString();
		}
		return array;
	}

	public static bool CheckIsSelectRight(string select, LevelData levelData)
	{
		if (string.IsNullOrEmpty(select) || levelData == null)
		{
			return false;
		}
		Word[] words = levelData.words;
		for (int i = 0; i < words.Length; i++)
		{
			if (words[i].word == select)
			{
				return true;
			}
		}
		return false;
	}

	public static bool CheckIsSelectFinish(LevelData levelData, PlayerLevelData playerLevelData)
	{
		if (levelData == null || playerLevelData == null)
		{
			return false;
		}
		List<string> selectWords = playerLevelData.selectWords;
		if (selectWords.IsEmpty())
		{
			return false;
		}
		Word[] words = levelData.words;
		for (int i = 0; i < words.Length; i++)
		{
			if (selectWords.IndexOf(words[i].word) == -1)
			{
				return false;
			}
		}
		return true;
	}

	public static bool CheckIsRepeatSelect(string word, PlayerLevelData playerLevelData)
	{
		if (playerLevelData == null)
		{
			return false;
		}
		List<string> selectWords = playerLevelData.selectWords;
		if (selectWords.IsEmpty())
		{
			return false;
		}
		return selectWords.IndexOf(word) != -1;
	}

	public static void OpenGameMainLevel(int level)
	{
		SingleObject<UIManager>.instance.OpenUI(UITypeDefine.GameMain, null, delegate(GameObject obj)
		{
			GameMainUICtrl component = obj.GetComponent<GameMainUICtrl>();
			component.DealyStartGame(0.1f, level);
			SingleObject<UIManager>.instance.Close(UITypeDefine.DatingUI);
		});
	}

	public static List<Word> GetUnSelectWord(LevelData levelData, PlayerLevelData playerLevelData)
	{
		if (levelData == null || playerLevelData == null)
		{
			return null;
		}
		List<string> selectWords = playerLevelData.selectWords;
		Word[] words = levelData.words;
		List<Word> list = new List<Word>();
		for (int i = 0; i < words.Length; i++)
		{
			if (selectWords.IndexOf(words[i].word) == -1)
			{
				list.Add(words[i]);
			}
		}
		return list;
	}

	public static int GetStarCountByPlayerLevelData(PlayerLevelData playerLevelData)
	{
		if (playerLevelData == null)
		{
			return 0;
		}
		if (playerLevelData.useCoinTimes == 0)
		{
			return 3;
		}
		if (playerLevelData.useCoinTimes < 2)
		{
			return 2;
		}
		return 1;
	}

	public static string[] GetSelectBounsWord()
	{
		string @string = PlayerPrefs.GetString("datakey_203", string.Empty);
		if (string.IsNullOrEmpty(@string))
		{
			return new string[0];
		}
		Hashtable data = @string.DecodeJson();
		ArrayList arrayList = data.GetArrayList("words");
		string[] array = new string[arrayList.Count];
		for (int i = 0; i < arrayList.Count; i++)
		{
			array[i] = arrayList[i] as string;
		}
		return array;
	}

	public static int GetNowOpenLevel()
	{
		int gameMaxLevel = WordConfig.gameMaxLevel;
		int num = 0;
		for (int i = 0; i < gameMaxLevel; i++)
		{
			PlayerLevelData byLevel = PlayerLevelData.GetByLevel(i + 1);
			if (byLevel.isPass)
			{
				num = i + 1;
				continue;
			}
			break;
		}
		if (num < gameMaxLevel)
		{
			return num + 1;
		}
		return gameMaxLevel;
	}
}
