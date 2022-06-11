using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LotteryUICtrl : MonoBehaviour
{
	private Transform _content;

	private Transform _gird;

	private Transform _targetPos;

	private int[] _coinCounts = new int[5] { 30, 40, 50, 60, 100 };

	private List<int> _selected = new List<int>();

	private List<Transform> _selectedItems = new List<Transform>();

	private bool _isSelectFinish;

	private void Start()
	{
		_content = base.transform.Find("Content");
		_gird = _content.Find("Grid");
		_targetPos = _content.Find("TargetPos");
		InitGrid();
		_content.GetComponent<Animation>().OrderPlay(string.Empty);
		SingleObject<SoundManager>.instance.PlayWordCuteSound(SoundType.enu_open);
	}

	private void InitGrid()
	{
		for (int i = 0; i < _gird.childCount; i++)
		{
			Transform item = _gird.GetChild(i);
			ShowItem(item, false);
			item.GetComponent<Button>().onClick.AddListener(delegate
			{
				OnSelectItem(item);
			});
		}
	}

	private void ShowItem(Transform item, bool isFace, int count = 0)
	{
		Transform transform = item.Find("QuesMark");
		Transform transform2 = item.Find("Coin");
		Text component = item.Find("Count").GetComponent<Text>();
		transform.gameObject.SetActive(!isFace);
		transform2.gameObject.SetActive(isFace);
		component.gameObject.SetActive(isFace);
		component.text = count.ToString();
	}

	private void OnSelectItem(Transform item)
	{
		if (_isSelectFinish || _selectedItems.IndexOf(item) != -1)
		{
			return;
		}
		_selectedItems.Add(item);
		iTween.ShakeScale(item.gameObject, new Vector3(0.1f, 0.1f, 1f), 0.5f);
		SingleObject<SchudleManger>.instance.Schudle(delegate
		{
			int nowGetCoin = RandomGetNumber();
			ShowItem(item, true, nowGetCoin);
			if (_selected.IndexOf(nowGetCoin) == -1)
			{
				_selected.Add(nowGetCoin);
			}
			else
			{
				SingleObject<SchudleManger>.instance.Schudle(delegate
				{
					SingleObject<SoundManager>.instance.PlayWordCuteSound(SoundType.reword_coin);
					_isSelectFinish = true;
					ShowAllItem(delegate
					{
						DoSelectFinish(nowGetCoin);
					});
				}, 0.5f);
			}
		}, 0.5f);
	}

	private int RandomGetNumber()
	{
		int num = Random.Range(0, _coinCounts.Length);
		return _coinCounts[num];
	}

	private void DoSelectFinish(int rewardCount)
	{
		RewardMessageBox.ShowAndSendReward(rewardCount, _targetPos.position, delegate
		{
			CloseUI();
		});
	}

	private void ShowAllItem(UnityAction callback)
	{
		Sequence sequence = DOTween.Sequence();
		for (int i = 0; i < _gird.childCount; i++)
		{
			Transform item = _gird.GetChild(i);
			if (_selectedItems.IndexOf(item) == -1)
			{
				int nowGetCoin = RandomGetNumber();
				sequence.AppendCallback(delegate
				{
					ShowItem(item, true, nowGetCoin);
					item.Find("Back").GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, 1f);
				});
				sequence.AppendInterval(0.1f);
			}
		}
		sequence.AppendInterval(1f);
		sequence.AppendCallback(delegate
		{
			callback.MyInvoke();
		});
		sequence.Play();
	}

	private void CloseUI()
	{
		SingleObject<SoundManager>.instance.PlayWordCuteSound(SoundType.menu_close);
		_content.GetComponent<Animation>().ReserverPlay(string.Empty);
		SingleObject<SchudleManger>.instance.Schudle(delegate
		{
			SingleObject<UIManager>.instance.Close(UITypeDefine.LotteryUI);
		}, 0.5f);
	}
}
