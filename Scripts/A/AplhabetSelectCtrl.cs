using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AplhabetSelectCtrl : MonoBehaviour
{
	public static AplhabetSelectCtrl instance;

	public bool canOpration = true;

	public UnityAction<string> onSelectFinish;

	private Transform _alphabetShow;

	private Text _showText;

	private Transform _alphabetCircle;

	private Transform _alphabetItemsParents;

	private PointerDonwUpListener _circleDownUpListener;

	private PointerMoveListener _circleMoveListener;

	private GameObject _alphabetItemPrefab;

	private LineRenderer _connectLine;

	private List<AlphabetItem> _alphabetItems = new List<AlphabetItem>();

	private string[] _alphabetData = new string[5] { "a", "b", "c", "d", "e" };

	private List<AlphabetItem> _selectedItems = new List<AlphabetItem>();

	private float _circleRedius;

	private bool _pointerIsDown;

	private Sequence _showTextSequence;

	private void Awake()
	{
		instance = this;
		_alphabetShow = base.transform.Find("AlphabetShow");
		_showText = _alphabetShow.Find("Text").GetComponent<Text>();
		_alphabetCircle = base.transform.Find("AlphabetCircle");
		_alphabetItemsParents = _alphabetCircle.Find("AlphabetItems");
		_alphabetItemPrefab = _alphabetItemsParents.Find("Item").gameObject;
		_connectLine = _alphabetCircle.Find("Line").GetComponent<LineRenderer>();
		_circleMoveListener = _alphabetCircle.GetComponent<PointerMoveListener>();
		_circleMoveListener.onPointerMove.AddListener(OnCirclePoinerMove);
		_circleDownUpListener = _alphabetCircle.GetComponent<PointerDonwUpListener>();
		_circleDownUpListener.onPointerDown.AddListener(OnCirclePointerDown);
		_circleDownUpListener.onPoinerUp.AddListener(OnCirclePoinerUp);
	}

	private void Start()
	{
		_alphabetCircle.localScale = new Vector3(0f, 0f, 1f);
	}

	public void InitSelect(string[] alphabetData)
	{
		_alphabetData = alphabetData;
		RectTransform rectTransform = _alphabetCircle as RectTransform;
		RectTransform component = _alphabetItemPrefab.GetComponent<RectTransform>();
		CaculatItemSize(component);
		_circleRedius = rectTransform.sizeDelta.x / 2f - 5f - component.sizeDelta.x / 2f;
		_alphabetItemPrefab.SetActive(false);
		InitCircle();
		InitCircleItemPos();
		ReSetAllItemsSelectState(false);
		UpdateShowText();
	}

	private void CaculatItemSize(RectTransform itemRect)
	{
		if (_alphabetData.Length >= 5)
		{
			float num = 1f - (float)(_alphabetData.Length - 5) * 1f / (float)_alphabetData.Length;
			itemRect.sizeDelta *= num;
			float widthMultiplier = _connectLine.widthMultiplier;
			_connectLine.widthMultiplier = widthMultiplier * num;
		}
	}

	private void InitCircle()
	{
		if (_alphabetData.IsEmpty())
		{
			return;
		}
		for (int i = 0; i < _alphabetData.Length; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(_alphabetItemPrefab);
			gameObject.transform.SetParent(_alphabetItemsParents, false);
			gameObject.SetActive(true);
			AlphabetItem item = new AlphabetItem(gameObject.transform, _alphabetData[i]);
			PointerInOutListener component = gameObject.GetComponent<PointerInOutListener>();
			component.onPointerIn.AddListener(delegate
			{
				OnAlphabetItemPointerIn(item);
			});
			component.onPoinerOut.AddListener(delegate
			{
				OnAlphabetItemPointerOut(item);
			});
			item.Init();
			_alphabetItems.Add(item);
		}
	}

	private void InitCircleItemPos(bool haveAnimatin = false)
	{
		if (!_alphabetItems.IsEmpty())
		{
			float num = 360f / (float)_alphabetItems.Count;
			for (int i = 0; i < _alphabetItems.Count; i++)
			{
				_alphabetItems[i].targetLocalPos = new Vector3(_circleRedius * Mathf.Sin(num * (float)i * ((float)Math.PI / 180f)), _circleRedius * Mathf.Cos(num * (float)i * ((float)Math.PI / 180f)), 0f);
				_alphabetItems[i].ToTargetPos(haveAnimatin);
			}
		}
	}

	private void UpdateShowText()
	{
		if (_selectedItems.Count == 0)
		{
			_alphabetShow.gameObject.SetActive(false);
			return;
		}
		ShowAlpHabetText();
		_showText.text = GetSelectString().ToUpper();
	}

	private void ShowAlpHabetText()
	{
		if (_showTextSequence != null)
		{
			_showTextSequence.Restart();
			_showTextSequence.Kill();
		}
		_alphabetShow.gameObject.SetActive(true);
	}

	private string GetSelectString()
	{
		string text = string.Empty;
		for (int i = 0; i < _selectedItems.Count; i++)
		{
			text += _selectedItems[i].data;
		}
		return text;
	}

	private void OnCirclePointerDown()
	{
		if ((WordCuteData.gameState == GameSate.Playing || WordCuteData.gameState == GameSate.TouristGuide) && canOpration && !_pointerIsDown)
		{
			_pointerIsDown = true;
			_selectedItems.Clear();
			MessageManger.SendMessage(50);
		}
	}

	private void OnCirclePoinerUp()
	{
		_pointerIsDown = false;
		_connectLine.gameObject.SetActive(false);
		ReSetAllItemsSelectState(false);
		string selectString = GetSelectString();
		onSelectFinish.MyInvoke(selectString);
		MessageManger.SendMessage(51, selectString);
	}

	private void OnCirclePoinerMove(Vector3 pointerWorldPos)
	{
		if (_pointerIsDown)
		{
			UpdateLine(pointerWorldPos);
		}
	}

	private void OnAlphabetItemPointerIn(AlphabetItem alphabetItem)
	{
		if (_pointerIsDown)
		{
			int num = _selectedItems.IndexOf(alphabetItem);
			if (num == -1)
			{
				_selectedItems.Add(alphabetItem);
				alphabetItem.DoSelect(true);
				DoOnSelectChange();
				int count = _selectedItems.Count;
				SingleObject<SoundManager>.instance.PlayWordCuteSound(SoundType.peak + count);
			}
			if (_selectedItems.Count > 1 && num == _selectedItems.Count - 2)
			{
				AlphabetItem alphabetItem2 = _selectedItems[num + 1];
				alphabetItem2.DoSelect(false);
				_selectedItems.RemoveAt(num + 1);
				DoOnSelectChange();
				SingleObject<SoundManager>.instance.PlayWordCuteSound(SoundType.dropback);
			}
		}
	}

	private void DoOnSelectChange()
	{
		UpdateShowText();
	}

	private void OnAlphabetItemPointerOut(AlphabetItem alphabetItem)
	{
		if (_pointerIsDown)
		{
		}
	}

	private void ReSetAllItemsSelectState(bool isSelect)
	{
		for (int i = 0; i < _alphabetItems.Count; i++)
		{
			_alphabetItems[i].DoSelect(isSelect);
		}
	}

	private void UpdateLine(Vector3 nowPos)
	{
		if (_selectedItems.IsEmpty())
		{
			_connectLine.gameObject.SetActive(false);
			return;
		}
		_connectLine.gameObject.SetActive(true);
		Vector3 vector = new Vector3(0f, 0f, 0.1f);
		List<Vector3> list = new List<Vector3>();
		for (int i = 0; i < _selectedItems.Count; i++)
		{
			list.Add(_selectedItems[i].obj.position - vector);
		}
		nowPos.Set(nowPos.x, nowPos.y, list[list.Count - 1].z);
		if (_selectedItems.Count != _alphabetItems.Count)
		{
			list.Add(nowPos);
		}
		_connectLine.positionCount = list.Count;
		_connectLine.SetPositions(list.ToArray());
	}

	public void ReDoItemPos()
	{
		CommonTools.RandomConfuseList(_alphabetItems);
		InitCircleItemPos(true);
	}

	public void DoSelectErrorThing(string word)
	{
		_showTextSequence = DOTween.Sequence();
		_showTextSequence.Append(_alphabetShow.DOShakePosition(WordConfig.duration, new Vector3(20f, 0f, 0f)));
		_showTextSequence.AppendCallback(delegate
		{
			_alphabetShow.gameObject.SetActive(false);
		});
	}

	public void DoSelectRepeatThing(string word)
	{
		_showTextSequence = DOTween.Sequence();
		_showTextSequence.Append(_alphabetShow.DOScale(new Vector3(1.2f, 1.2f, 1f), 0.1f));
		_showTextSequence.Append(_alphabetShow.DOScale(Vector3.one, 0.1f));
		_showTextSequence.Append(_alphabetShow.DOScale(new Vector3(1.2f, 1.2f, 1f), 0.1f));
		_showTextSequence.Append(_alphabetShow.DOScale(Vector3.one, 0.1f));
		_showTextSequence.Append(_alphabetShow.DOScale(new Vector3(1.2f, 1.2f, 1f), 0.1f));
		_showTextSequence.Append(_alphabetShow.DOScale(Vector3.one, 0.1f));
		_showTextSequence.AppendCallback(delegate
		{
			_alphabetShow.gameObject.SetActive(false);
		});
	}

	public void DoSelectRightThing(string word)
	{
		AplhabetTableCtrl.instance.FlyWordItems(word, _showText.transform.position);
		_alphabetShow.gameObject.SetActive(false);
	}

	public void HiddenShowText()
	{
		_alphabetShow.gameObject.SetActive(false);
	}

	public void PlayAnimation(bool isOpen)
	{
		if (isOpen)
		{
			_alphabetCircle.DOScale(1f, WordConfig.duration).SetEase(Ease.OutBack);
		}
		else
		{
			_alphabetCircle.DOScale(0f, WordConfig.duration).SetEase(Ease.InBack);
		}
	}

	public Vector3 GetShowTextPos()
	{
		return _alphabetShow.transform.position;
	}

	public AlphabetItem GetItemByAlphabet(string alphabet)
	{
		if (_alphabetItems.IsEmpty())
		{
			return null;
		}
		for (int i = 0; i < _alphabetItems.Count; i++)
		{
			if (_alphabetItems[i].data.ToLower() == alphabet.ToLower())
			{
				return _alphabetItems[i];
			}
		}
		return null;
	}
}
