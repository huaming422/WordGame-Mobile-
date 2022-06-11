using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameChargeUICtrl : MonoBehaviour
{
	private UnityAction<int, int> onBuyFinish;

	private Transform _content;

	private Transform _buttonList;

	private int _currentType = 1;

	private int _nowSelectIndex = 2;

	private bool isShowRemoveButton = true;

	private void Awake()
	{
		_content = base.transform.Find("Content");
		_content.Find("Close").GetComponent<Button>().onClick.AddListener(OnClickCloseButton);
		_buttonList = _content.Find("GoodsList");
		InitUI();
	}

	private void Start()
	{
		SingleObject<SoundManager>.instance.PlayWordCuteSound(SoundType.enu_open);
		_content.GetComponent<Animation>().OrderPlay(string.Empty);
	}

	private void InitUI()
	{
		for (int i = 0; i < _buttonList.childCount; i++)
		{
			int temp = i;
			Transform child = _buttonList.GetChild(i);
			Button component = child.Find("Button").GetComponent<Button>();
			component.onClick.AddListener(delegate
			{
				OnClickBuyButton(temp);
			});
		}
		_buttonList.GetChild(_buttonList.childCount - 1).gameObject.SetActive(!PlayerData.i.playerBuyRemoveAd);
	}

	private void OnClickBuyButton(int index)
	{
		if (index == 4)
		{
			SingleObject<UnityInAppPurchManager>.instance.Purch(5.ToString(), delegate(bool isOk, string id)
			{
				BuyCallback(isOk, id, 5);
			});
		}
		else
		{
			SingleObject<UnityInAppPurchManager>.instance.Purch((index + 1).ToString(), delegate(bool isOk, string id)
			{
				BuyCallback(isOk, id, index + 1);
			});
		}
	}

	private void BuyCallback(bool isOk, string id, int selectIndex)
	{
		if (!isOk)
		{
			Util.LogWarning(id);
			return;
		}
		if (selectIndex == 5)
		{
			BuyRemoveAd();
		}
		if (selectIndex <= 4 && selectIndex >= 1)
		{
			BuyTools(selectIndex);
		}
	}

	private void BuyRemoveAd()
	{
		PlayerData.i.playerBuyRemoveAd = true;
		PlayerData.Save();
		_buttonList.GetChild(_buttonList.childCount - 1).gameObject.SetActive(false);
		SingleObject<UnityInAppPurchManager>.instance.AddGoodPurch(5.ToString());
	}

	private int IndexToCoinCount(int index)
	{
		switch (index)
		{
		case 1:
			return 300;
		case 2:
			return 1500;
		case 3:
			return 3600;
		case 4:
			return 10000;
		default:
			return 0;
		}
	}

	private void BuyTools(int index)
	{
		SingleObject<SoundManager>.instance.PlayWordCuteSound(SoundType.coin_increse);
		int num = IndexToCoinCount(index);
		PlayerData.i.coinCount += num;
		PlayerData.Save();
		if (onBuyFinish != null)
		{
			onBuyFinish(_currentType, num);
		}
	}

	private void OnClickCloseButton()
	{
		SingleObject<SoundManager>.instance.PlayWordCuteSound(SoundType.menu_close);
		_content.GetComponent<Animation>().ReserverPlay(string.Empty);
		SingleObject<SchudleManger>.instance.Schudle(delegate
		{
			SingleObject<UIManager>.instance.Close(UITypeDefine.GameChargeUI);
		}, 0.5f);
	}

	public static void Show(UnityAction<int, int> onBuyFinish = null)
	{
		SingleObject<UIManager>.instance.OpenUI(UITypeDefine.GameChargeUI, null, delegate(GameObject obj)
		{
			GameChargeUICtrl component = obj.GetComponent<GameChargeUICtrl>();
			component.onBuyFinish = onBuyFinish;
		});
	}
}
