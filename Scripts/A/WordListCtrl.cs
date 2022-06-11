using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordListCtrl : MonoBehaviour, IFixCountScrollDataCtrl
{
	public static WordListCtrl instance;

	private List<WordLine> _result;

	private FixCountScrollRect _fixCountRect;

	public bool CanCreateChild(int index)
	{
		if (_result == null)
		{
			return false;
		}
		return index < _result.Count && index >= 0;
	}

	public bool CanUpdateChildItemDataIndex(int index)
	{
		if (_result == null)
		{
			return false;
		}
		return index < _result.Count && index >= 0;
	}

	public void CreateChildItem(int index, GameObject obj)
	{
		InitWordItem(index, obj.transform);
	}

	public void UpdateChildItemDataIndex(int index, GameObject obj)
	{
		InitWordItem(index, obj.transform);
	}

	public void UpdateChildItemPosIndex(int posIndex, GameObject obj)
	{
	}

	private void InitWordItem(int index, Transform lineItem)
	{
		if (index < _result.Count)
		{
			WordLine line = _result[index];
			Text component = lineItem.Find("Word").GetComponent<Text>();
			Text component2 = lineItem.Find("length").GetComponent<Text>();
			Text component3 = lineItem.Find("Trans").GetComponent<Text>();
			Button component4 = lineItem.Find("HoriUse").GetComponent<Button>();
			Button component5 = lineItem.Find("VertUse").GetComponent<Button>();
			Button component6 = lineItem.Find("Bouns").GetComponent<Button>();
			component4.onClick.RemoveAllListeners();
			component5.onClick.RemoveAllListeners();
			component6.onClick.RemoveAllListeners();
			component4.onClick.AddListener(delegate
			{
				OnClickHoriUse(line.word);
			});
			component5.onClick.AddListener(delegate
			{
				OnClickVertUse(line.word);
			});
			component.text = line.word;
			component2.text = line.length.ToString();
			component3.text = line.trans;
		}
	}

	private void Start()
	{
		instance = this;
		_fixCountRect = GetComponent<FixCountScrollRect>();
		_fixCountRect.RegistDataCtrl(this);
	}

	public void InitList(List<WordLine> words)
	{
		_result = words;
		_fixCountRect.ReInit();
	}

	private void OnClickHoriUse(string word)
	{
		DisignGridCtrl.instance.SetTextByWord(word, true);
	}

	private void OnClickVertUse(string word)
	{
		DisignGridCtrl.instance.SetTextByWord(word, false);
	}
}
