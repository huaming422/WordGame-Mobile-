using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class QuitUICtrl : MonoBehaviour
{
	private Transform _content;

	private Transform _background;

	private Button _yesButton;

	private Button _noButton;

	private UnityAction<bool> _callback;

	private int _showType = 1;

	private void Start()
	{
		_content = base.transform.Find("Content");
		_background = _content.Find("Background");
		_background.Find(_showType.ToString()).gameObject.SetActive(true);
		_yesButton = _content.Find("Yes").GetComponent<Button>();
		_noButton = _content.Find("No").GetComponent<Button>();
		_yesButton.onClick.AddListener(OnClickYes);
		_noButton.onClick.AddListener(OnClickNo);
		_content.GetComponent<Animation>().OrderPlay(string.Empty);
		SingleObject<SoundManager>.instance.PlayWordCuteSound(SoundType.enu_open);
	}

	private void OnClickNo()
	{
		CloseUI(false);
	}

	private void OnClickYes()
	{
		CloseUI(true);
	}

	private void CloseUI(bool isOk)
	{
		SingleObject<SoundManager>.instance.PlayWordCuteSound(SoundType.menu_close);
		_content.GetComponent<Animation>().ReserverPlay(string.Empty);
		SingleObject<SchudleManger>.instance.Schudle(delegate
		{
			SingleObject<UIManager>.instance.Close(UITypeDefine.QuitUI);
			_callback.MyInvoke(isOk);
		}, 0.5f);
	}

	private void OnClickJumpMark()
	{
		Application.OpenURL("amzn://apps/android?p=com.sagafun.bingolove&amp;s=bingo%20games");
	}

	public static void OpenUI(int type, UnityAction<bool> callback)
	{
		SingleObject<UIManager>.instance.OpenUI(UITypeDefine.QuitUI, null, delegate(GameObject obj)
		{
			QuitUICtrl component = obj.GetComponent<QuitUICtrl>();
			component._showType = type;
			component._callback = callback;
		});
	}
}
