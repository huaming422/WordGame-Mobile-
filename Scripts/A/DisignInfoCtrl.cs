using UnityEngine;
using UnityEngine.UI;

public class DisignInfoCtrl : MonoBehaviour
{
	public static DisignInfoCtrl instance;

	private Text _loginfo;

	private Text _useAlphabet;

	private Text _useWords;

	private Text _gridSize;

	private Text _repeatUse;

	private void Awake()
	{
		instance = this;
		_loginfo = base.transform.Find("LogText").GetComponent<Text>();
		_useAlphabet = base.transform.Find("UseAlphabet").GetComponent<Text>();
		_useWords = base.transform.Find("UseWords").GetComponent<Text>();
		_gridSize = base.transform.Find("Size").GetComponent<Text>();
		_repeatUse = base.transform.Find("RepeatUse").GetComponent<Text>();
	}

	public void Log(string content)
	{
		_loginfo.text = content;
	}

	public void LogError(string content)
	{
		_loginfo.text = "<color=red>" + content + "</color>";
	}

	public void Clearn()
	{
		_loginfo.text = string.Empty;
	}

	public void ShowUseAlphabet(string alphabets)
	{
		if (!string.IsNullOrEmpty(alphabets))
		{
			string text = alphabets[0].ToString();
			for (int i = 1; i < alphabets.Length; i++)
			{
				text = text + ", " + alphabets[i];
			}
			_useAlphabet.text = text;
		}
	}

	public void ShowUseWords(Word[] words)
	{
		if (!words.IsEmpty())
		{
			string text = words[0].word.ToLower();
			for (int i = 1; i < words.Length; i++)
			{
				text = text + ", " + words[i].word.ToLower();
			}
			_useWords.text = text;
		}
	}

	public void ShowUseGridSize(int hori, int vert)
	{
		_gridSize.text = hori + " X " + vert;
	}
}
