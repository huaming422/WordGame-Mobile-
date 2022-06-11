using System.Collections;
using System.Collections.Generic;

public class Word
{
	public string word = string.Empty;

	public AlpHabetPos[] alphabetPos;

	public string ToJosn()
	{
		Hashtable hashtable = new Hashtable();
		hashtable["W"] = word;
		List<string> list = new List<string>();
		for (int i = 0; i < alphabetPos.Length; i++)
		{
			list.Add(alphabetPos[i].ToString());
		}
		hashtable["A"] = list;
		return hashtable.ToJson();
	}

	public static Word ToWord(string json)
	{
		Hashtable data = json.DecodeJson();
		Word word = new Word();
		word.word = data.GetString("W");
		ArrayList arrayList = data.GetArrayList("A");
		AlpHabetPos[] array = new AlpHabetPos[arrayList.Count];
		for (int i = 0; i < arrayList.Count; i++)
		{
			array[i] = AlpHabetPos.ToAlpHabetPos(arrayList[i] as string);
		}
		word.alphabetPos = array;
		return word;
	}
}
