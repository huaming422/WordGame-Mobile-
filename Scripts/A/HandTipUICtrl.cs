using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HandTipUICtrl : MonoBehaviour
{
	public static HandTipUICtrl instance;

	private Transform _content;

	private Transform _step1;

	private Transform _step2;

	private Button _closeButton;

	private Button _cancelButton;

	private Button _confirButto;

	private AlphabetTableItem _nowSelectItem;

	private void Start()
	{
		instance = this;
		_content = base.transform.Find("Content");
		_step1 = _content.Find("Step1");
		_step2 = _content.Find("Step2");
		_closeButton = _step1.Find("Close").GetComponent<Button>();
		_cancelButton = _step2.Find("No").GetComponent<Button>();
		_confirButto = _step2.Find("Yes").GetComponent<Button>();
		_closeButton.onClick.AddListener(OnClickClose);
		_cancelButton.onClick.AddListener(OnClickCancel);
		_confirButto.onClick.AddListener(OnClickConfir);
		ShowType(1);
	}

	public void ShowType(int step)
	{
		_step1.gameObject.SetActive(step == 1);
		_step2.gameObject.SetActive(step == 2);
		if (step == 2)
		{
			_step2.GetComponent<Animation>().OrderPlay(string.Empty);
		}
	}

	public void DoSelectItem(AlphabetTableItem item)
	{
		if (item != null && item.state == 1 && _nowSelectItem == null)
		{
			item.DoTipChangeBack(true);
			_nowSelectItem = item;
			ShowType(2);
		}
	}

	private void OnClickClose()
	{
		WordCuteData.gameState = GameSate.Playing;
		SingleObject<UIManager>.instance.Close(UITypeDefine.UseHandTipUI);
	}

	private void OnClickCancel()
	{
		if (_nowSelectItem != null)
		{
			_nowSelectItem.DoTipChangeBack(false);
			_nowSelectItem = null;
		}
		WordCuteData.gameState = GameSate.Playing;
		CloseUI();
	}

	private void OnClickConfir()
	{
		WordCuteData.gameState = GameSate.Playing;
		PlayerDataManager.ChangeCoin(-WordConfig.diyTipCoinCount);
		AplhabetTableCtrl.instance.SelectItem(_nowSelectItem);
		CloseUI();
	}

	private void CloseUI(UnityAction callback = null)
	{
		_step2.GetComponent<Animation>().ReserverPlay(string.Empty);
		SingleObject<SchudleManger>.instance.Schudle(delegate
		{
			callback.MyInvoke();
			SingleObject<UIManager>.instance.Close(UITypeDefine.UseHandTipUI);
		}, 0.5f);
	}
}
