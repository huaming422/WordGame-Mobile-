using System.Collections.Generic;
using System.Text.RegularExpressions;

public class DesignCommonTools
{
	public static bool CheckHaveOtherSymbol(string src)
	{
		string pattern = "[^a-z]";
		return Regex.IsMatch(src, pattern);
	}

	public static string ReplaceFirstOtherSymbol(string src)
	{
		string pattern = "^[^a-z]";
		return Regex.Replace(src, pattern, string.Empty);
	}

	public static string FilteSQLStr(string Str)
	{
		Str = Str.Replace("'", string.Empty);
		Str = Str.Replace("\"", string.Empty);
		Str = Str.Replace("&", "&");
		Str = Str.Replace("<", "<");
		Str = Str.Replace(">", ">");
		Str = Str.Replace("delete", string.Empty);
		Str = Str.Replace("update", string.Empty);
		Str = Str.Replace("insert", string.Empty);
		return Str;
	}

	private static void AddToAleryUsedWords(Dictionary<int, List<string>> aleryUseWords, LevelData levelData)
	{
		if (levelData == null || levelData.words == null)
		{
			return;
		}
		Word[] words = levelData.words;
		for (int i = 0; i < words.Length; i++)
		{
			int length = words[i].word.Length;
			List<string> list = null;
			if (aleryUseWords.ContainsKey(length))
			{
				list = aleryUseWords[length];
			}
			else
			{
				list = new List<string>();
				aleryUseWords.Add(length, list);
			}
			list.Add(words[i].word);
		}
	}

	public static bool CheckNowWordIsUsed(string word, Dictionary<int, List<string>> aleryUseWords)
	{
		int length = word.Length;
		List<string> value = null;
		if (!aleryUseWords.TryGetValue(length, out value))
		{
			return false;
		}
		return value.IndexOf(word) != -1;
	}

	public static int GetWorkMaxCountByLevel(int level)
	{
		if (level <= 5)
		{
			return 3;
		}
		if (level <= 15)
		{
			return 4;
		}
		if (level <= 45)
		{
			return 5;
		}
		if (level <= 100)
		{
			return 6;
		}
		if (level <= 250)
		{
			return 7;
		}
		if (level <= 350)
		{
			return 8;
		}
		if (level <= 550)
		{
			return 9;
		}
		if (level <= 850)
		{
			return 10;
		}
		if (level <= 1150)
		{
			return 11;
		}
		return 12;
	}
}
