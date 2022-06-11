using System.Collections;
using System.Collections.Generic;

public class LevelData
{
	public int gridHoriCount;

	public int gridVertCount;

	public Word[] words;

	public string useAlphabets = string.Empty;

	public LevelData()
	{
	}

	public LevelData(int hori, int vert, Word[] words)
	{
		gridHoriCount = hori;
		gridVertCount = vert;
		this.words = words;
	}

	public string ToJson()
	{
		Hashtable hashtable = new Hashtable();
		hashtable["H"] = gridHoriCount;
		hashtable["V"] = gridVertCount;
		hashtable["U"] = useAlphabets;
		List<string> list = new List<string>();
		for (int i = 0; i < words.Length; i++)
		{
			list.Add(words[i].ToJosn());
		}
		hashtable["W"] = list;
		return hashtable.ToJson();
	}

	public static LevelData ToLevelData(string json)
	{
		Hashtable data = json.DecodeJson();
		LevelData levelData = new LevelData();
		levelData.gridHoriCount = data.GetInt("H");
		levelData.gridVertCount = data.GetInt("V");
		levelData.useAlphabets = data.GetString("U");
		ArrayList arrayList = data.GetArrayList("W");
		Word[] array = new Word[arrayList.Count];
		for (int i = 0; i < arrayList.Count; i++)
		{
			array[i] = Word.ToWord(arrayList[i] as string);
		}
		levelData.words = array;
		return levelData;
	}

	public Dictionary<int, string> GetPosToAlphabet()
	{
		Dictionary<int, string> dictionary = new Dictionary<int, string>();
		if (words == null)
		{
			return null;
		}
		for (int i = 0; i < words.Length; i++)
		{
			Word word = words[i];
			AlpHabetPos[] alphabetPos = word.alphabetPos;
			for (int j = 0; j < alphabetPos.Length; j++)
			{
				AlpHabetPos alpHabetPos = alphabetPos[j];
				int key = alpHabetPos.v * gridHoriCount + alpHabetPos.h;
				if (!dictionary.ContainsKey(key))
				{
					dictionary.Add(key, word.word[j].ToString());
				}
			}
		}
		return dictionary;
	}
}
