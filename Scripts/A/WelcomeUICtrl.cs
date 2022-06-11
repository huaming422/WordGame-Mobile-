using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WelcomeUICtrl : MonoBehaviour
{
	private Transform _content;

	private Button _acceptButton;

	private Button _termsButton;

	private Button _privacyPolicy;

	private Button _closeUI;

	private UnityAction<bool> _onfinish;

	private void Start()
	{
		_content = base.transform.Find("Content");
		_acceptButton = _content.Find("Accept").GetComponent<Button>();
		_termsButton = _content.Find("Terms").GetComponent<Button>();
		_privacyPolicy = _content.Find("privacy").GetComponent<Button>();
		_closeUI = _content.Find("Close").GetComponent<Button>();
		_acceptButton.onClick.AddListener(OnClickAccept);
		_termsButton.onClick.AddListener(OnClickTerms);
		_privacyPolicy.onClick.AddListener(OnClickPrivacy);
		_closeUI.onClick.AddListener(OnCloseUI);
		_content.GetComponent<Animation>().OrderPlay(string.Empty);
	}

	private void OnClickAccept()
	{
		_content.GetComponent<Animation>().ReserverPlay(string.Empty);
		PlayerDataManager.isAcceptPrivate = true;
		SingleObject<SchudleManger>.instance.Schudle(delegate
		{
			_onfinish.MyInvoke(true);
			SingleObject<UIManager>.instance.Close(UITypeDefine.WelcomeUI);
		}, 0.5f);
	}

	private void OnClickTerms()
	{
		Application.OpenURL("http://www.sagafun.com/terms.html");
	}

	private void OnClickPrivacy()
	{
		Application.OpenURL("http://www.sagafun.com/policy.html");
	}

	public static void Show(UnityAction<bool> callback = null)
	{
		if (!PlayerDataManager.isAcceptPrivate)
		{
			SingleObject<UIManager>.instance.OpenUI(UITypeDefine.WelcomeUI, null, delegate(GameObject obj)
			{
				WelcomeUICtrl component = obj.GetComponent<WelcomeUICtrl>();
				component._onfinish = callback;
			});
		}
	}

	private void OnCloseUI()
	{
		Application.Quit();
	}
}
