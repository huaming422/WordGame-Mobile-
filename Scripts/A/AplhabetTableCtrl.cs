using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class AplhabetTableCtrl : MonoBehaviour
{
	public static AplhabetTableCtrl instance;

	private Transform _itemsContaner;

	private GameObject _itemPrefab;

	private AlphabetGirdCtrl _gridCtrl;

	private Dictionary<string, List<AlphabetTableItem>> _wordToItemList = new Dictionary<string, List<AlphabetTableItem>>();

	public Vector2 gridNodeSize
	{
		get
		{
			if (_gridCtrl == null)
			{
				return Vector2.zero;
			}
			return new Vector2(_gridCtrl.nodeWidth, _gridCtrl.nodeWidth);
		}
	}

	private void Awake()
	{
		instance = this;
		_itemsContaner = base.transform.Find("AlphabetItems");
		_itemPrefab = _itemsContaner.Find("Item").gameObject;
		_gridCtrl = _itemsContaner.GetComponent<AlphabetGirdCtrl>();
	}

	public void InitGridTable(LevelData levelData)
	{
		_gridCtrl.InitGrid(levelData.gridHoriCount, levelData.gridVertCount);
		Dictionary<int, string> posIndexToAlphabet = levelData.GetPosToAlphabet();
		_gridCtrl.OprationAllNodes(delegate(AlphabetTableItem node)
		{
			int key = node.vertPosIndex * levelData.gridHoriCount + node.horiPosIndex;
			if (posIndexToAlphabet.ContainsKey(key))
			{
				node.obj = CreateItem();
				node.data = posIndexToAlphabet[key];
				node.Init();
				node.Hidden();
				node.AddClickEvent(OnClickItem);
			}
		});
		BuildWordToItemList(levelData);
		InitShowPlayerLevelData();
	}

	private Transform CreateItem()
	{
		GameObject gameObject = Object.Instantiate(_itemPrefab);
		gameObject.SetActive(true);
		gameObject.transform.SetParent(_itemsContaner, false);
		return gameObject.transform;
	}

	private void BuildWordToItemList(LevelData levelData)
	{
		Word[] words = levelData.words;
		int gridHoriCount = levelData.gridHoriCount;
		foreach (Word word in words)
		{
			AlpHabetPos[] alphabetPos = word.alphabetPos;
			List<AlphabetTableItem> list = new List<AlphabetTableItem>();
			for (int j = 0; j < alphabetPos.Length; j++)
			{
				AlpHabetPos alpHabetPos = alphabetPos[j];
				int posIndex = alpHabetPos.v * gridHoriCount + alpHabetPos.h;
				AlphabetTableItem itemByPosIndex = _gridCtrl.GetItemByPosIndex(posIndex);
				list.Add(itemByPosIndex);
			}
			if (!_wordToItemList.ContainsKey(word.word))
			{
				_wordToItemList.Add(word.word, list);
			}
		}
	}

	private void InitShowPlayerLevelData()
	{
		if (WordCuteData.playerLevelData == null)
		{
			return;
		}
		List<string> selectWords = WordCuteData.playerLevelData.selectWords;
		for (int i = 0; i < selectWords.Count; i++)
		{
			if (_wordToItemList.ContainsKey(selectWords[i]))
			{
				List<AlphabetTableItem> list = _wordToItemList[selectWords[i]];
				for (int j = 0; j < list.Count; j++)
				{
					list[j].DoSelect(true);
					list[j].ChangeSate(2);
				}
			}
		}
		List<int> specilIndexs = WordCuteData.playerLevelData.specilIndexs;
		for (int k = 0; k < specilIndexs.Count; k++)
		{
			AlphabetTableItem itemByPosIndex = _gridCtrl.GetItemByPosIndex(specilIndexs[k]);
			if (itemByPosIndex != null && !(itemByPosIndex.obj == null))
			{
				itemByPosIndex.ChangeSate(3);
				itemByPosIndex.DoTip();
			}
		}
	}

	public bool CheckIsSelectFinish()
	{
		List<AlphabetTableItem> allNodes = _gridCtrl.GetAllNodes();
		for (int i = 0; i < allNodes.Count; i++)
		{
			if (allNodes[i].state == 1)
			{
				return false;
			}
		}
		return true;
	}

	public bool RandomTip()
	{
		List<Word> unSelectWord = WordCuteTools.GetUnSelectWord(WordCuteData.nowLevelData, WordCuteData.playerLevelData);
		if (unSelectWord.IsEmpty())
		{
			return false;
		}
		List<AlphabetTableItem> list = new List<AlphabetTableItem>();
		for (int i = 0; i < unSelectWord.Count; i++)
		{
			if (!_wordToItemList.ContainsKey(unSelectWord[i].word))
			{
				continue;
			}
			List<AlphabetTableItem> list2 = _wordToItemList[unSelectWord[i].word];
			for (int j = 0; j < list2.Count; j++)
			{
				if (list2[j].state != 3 && list2[j].state != 2)
				{
					list.Add(list2[j]);
				}
			}
		}
		if (list.IsEmpty())
		{
			return false;
		}
		int index = Random.Range(0, list.Count);
		AlphabetTableItem item = list[index];
		item.ChangeSate(3);
		WordCuteData.playerLevelData.AddSpecilIndex(item.vertPosIndex * _gridCtrl.hori + item.horiPosIndex, false);
		WordCuteData.playerLevelData.AddUseCoinTimes();
		AlphabetItem itemByAlphabet = AplhabetSelectCtrl.instance.GetItemByAlphabet(item.data);
		InitFlyWordItem(item.data.ToUpper(), itemByAlphabet.obj.position, item.obj.position, delegate(GameObject obj)
		{
			if (obj != null && item.obj != null)
			{
				Object.Destroy(obj);
				item.DoSelect(true);
				if (CheckIsSelectFinish())
				{
					MessageManger.SendMessage(101);
				}
			}
		});
		return true;
	}

	public void DoWrodSelect(string word)
	{
		if (!_wordToItemList.ContainsKey(word))
		{
			return;
		}
		List<AlphabetTableItem> list = _wordToItemList[word];
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].state == 3)
			{
				int index = list[i].vertPosIndex * _gridCtrl.hori + list[i].horiPosIndex;
				WordCuteData.playerLevelData.RemoveSpecilIndex(index);
			}
			list[i].ChangeSate(2);
		}
	}

	public void FlyWordItems(string word, Vector3 startPos)
	{
		if (!_wordToItemList.ContainsKey(word))
		{
			return;
		}
		List<AlphabetTableItem> list = _wordToItemList[word];
		for (int i = 0; i < list.Count; i++)
		{
			AlphabetTableItem item = list[i];
			InitFlyWordItem(word[i].ToString(), startPos, item.obj.position, delegate(GameObject obj)
			{
				if (obj != null && item.obj != null)
				{
					Object.Destroy(obj);
					item.DoSelect(true);
				}
			});
		}
	}

	private void InitFlyWordItem(string alphabet, Vector3 startPos, Vector3 endPos, UnityAction<GameObject> callback)
	{
		Transform temp = Object.Instantiate(_itemPrefab).transform;
		temp.gameObject.SetActive(true);
		temp.SetParent(FlyObjMananger.instance.transform, false);
		AlphabetTableItem alphabetTableItem = new AlphabetTableItem(temp, alphabet);
		alphabetTableItem.width = gridNodeSize.x;
		alphabetTableItem.height = gridNodeSize.y;
		alphabetTableItem.Init();
		alphabetTableItem.DoSelect(true);
		temp.position = startPos;
		temp.localScale = new Vector3(0.2f, 0.2f, 1f);
		temp.DOMove(endPos, WordConfig.duration);
		temp.DOScale(Vector3.one, WordConfig.duration);
		SingleObject<SchudleManger>.instance.Schudle(delegate
		{
			if (callback != null)
			{
				callback(temp.gameObject);
			}
		}, WordConfig.duration);
	}

	public void PlayRepeatSelctWord(string word)
	{
		if (_wordToItemList.ContainsKey(word))
		{
			List<AlphabetTableItem> list = _wordToItemList[word];
			for (int i = 0; i < list.Count; i++)
			{
				AlphabetTableItem alphabetTableItem = list[i];
				alphabetTableItem.obj.DOShakeRotation(WordConfig.duration, WordConfig.shakeRotaion, 100);
			}
		}
	}

	public void PlayAnimation(bool isOpen)
	{
		if (_gridCtrl == null)
		{
			return;
		}
		List<AlphabetTableItem> allNodes = _gridCtrl.GetAllNodes();
		if (allNodes.IsEmpty())
		{
			return;
		}
		float num = 0.5f / (float)_gridCtrl.hori;
		float num2 = 0.5f / (float)_gridCtrl.vert;
		Vector3 vector = new Vector3(0f, 300f, 0f);
		for (int i = 0; i < allNodes.Count; i++)
		{
			Transform obj = allNodes[i].obj;
			CanvasGroup component = obj.GetComponent<CanvasGroup>();
			if (isOpen)
			{
				Sequence sequence = DOTween.Sequence();
				sequence.AppendInterval((float)allNodes[i].horiPosIndex * num + (float)allNodes[i].vertPosIndex * num2);
				sequence.Append(component.DOFade(1f, WordConfig.duration));
				sequence.Join(obj.DOScale(1f, WordConfig.duration));
				sequence.Play();
			}
			else
			{
				Vector3 endValue = obj.localPosition - vector;
				Sequence sequence2 = DOTween.Sequence();
				sequence2.AppendInterval(1f - ((float)allNodes[i].horiPosIndex * num + (float)allNodes[i].vertPosIndex * num2));
				sequence2.Append(component.DOFade(0f, WordConfig.duration));
				sequence2.Join(obj.DOLocalMove(endValue, WordConfig.duration));
				sequence2.Play();
			}
		}
	}

	private void OnClickItem(AlphabetTableItem item)
	{
		if (WordCuteData.gameState == GameSate.WaitUseTipHand)
		{
			HandTipUICtrl.instance.DoSelectItem(item);
		}
	}

	public void SelectItem(AlphabetTableItem item)
	{
		if (item != null && !(item.obj == null) && item.state == 1)
		{
			item.DoTip();
			item.ChangeSate(3);
			WordCuteData.playerLevelData.AddSpecilIndex(item.vertPosIndex * _gridCtrl.hori + item.horiPosIndex, false);
			WordCuteData.playerLevelData.AddUseCoinTimes();
			if (CheckIsSelectFinish())
			{
				MessageManger.SendMessage(101);
			}
		}
	}

	public void FlyBounsWord(string word, Vector3 startPos, Vector3 endPos, UnityAction callback = null)
	{
		float localStartX = (0f - (float)word.Length * gridNodeSize.x) / 2f + gridNodeSize.x / 2f;
		float timeInterVal = 0.5f / (float)word.Length;
		for (int i = 0; i < word.Length; i++)
		{
			InitFlyBounsWord(word[i].ToString(), startPos, endPos, localStartX, i, timeInterVal, delegate(int index)
			{
				if (index == word.Length - 1)
				{
					callback.MyInvoke();
				}
			});
		}
	}

	private void InitFlyBounsWord(string alphabet, Vector3 startPos, Vector3 endPos, float localStartX, int index, float timeInterVal, UnityAction<int> callback = null)
	{
		Transform temp = Object.Instantiate(_itemPrefab).transform;
		temp.gameObject.SetActive(true);
		temp.SetParent(FlyObjMananger.instance.transform, false);
		AlphabetTableItem alphabetTableItem = new AlphabetTableItem(temp, alphabet);
		alphabetTableItem.width = gridNodeSize.x;
		alphabetTableItem.height = gridNodeSize.y;
		alphabetTableItem.Init();
		alphabetTableItem.DoSelect(true);
		temp.position = startPos;
		temp.localScale = new Vector3(0.2f, 0.2f, 1f);
		SingleObject<SchudleManger>.instance.Schudle(delegate
		{
			Sequence sequence = DOTween.Sequence();
			Vector3 localPosition = temp.localPosition;
			Vector3 endValue = new Vector3(localStartX + (float)index * gridNodeSize.x, localPosition.y + 200f - gridNodeSize.y / 2f, localPosition.z);
			sequence.Append(temp.DOLocalMove(endValue, 0.3f));
			sequence.Join(temp.DOScale(Vector3.one, 0.3f));
			sequence.AppendInterval(0.5f + timeInterVal * (float)index);
			sequence.Append(temp.DOMove(endPos, 0.3f));
			sequence.AppendCallback(delegate
			{
				if (temp != null)
				{
					Object.Destroy(temp.gameObject);
				}
				callback.MyInvoke(index);
			});
			sequence.Play();
		}, 0.1f);
	}
}
