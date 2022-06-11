using UnityEngine;
using UnityEngine.UI;

public class SetUICtrl : MonoBehaviour
{
	private Transform _content;

	private Button _feedbackButton;

	private Button _musicButton;

	private Button _soundButton;

	private Button _closeButton;

	private Button _homeButton;

	public int showType = 1;

	private void Start()
	{
		SingleObject<SoundManager>.instance.PlayWordCuteSound(SoundType.enu_open);
		_content = base.transform.Find("Content");
		_feedbackButton = _content.Find("Feedback").GetComponent<Button>();
		_musicButton = _content.Find("Music").GetComponent<Button>();
		_soundButton = _content.Find("Sound").GetComponent<Button>();
		_closeButton = _content.Find("Close").GetComponent<Button>();
		_homeButton = _content.Find("Home").GetComponent<Button>();
		_feedbackButton.onClick.AddListener(OnClickFeedBack);
		_musicButton.onClick.AddListener(OnClickMusic);
		_soundButton.onClick.AddListener(OnClickSound);
		_closeButton.onClick.AddListener(CloseUI);
		_homeButton.onClick.AddListener(OnClickHome);
		_content.GetComponent<Animation>().OrderPlay(string.Empty);
		InitUI();
	}

	private void InitUI()
	{
		_feedbackButton.gameObject.SetActive(showType == 1);
		_homeButton.gameObject.SetActive(showType == 2);
		ProjectCommonFunction.UpdateButton(_musicButton.transform, SingleObject<SoundManager>.instance.CanPlayMusic());
		ProjectCommonFunction.UpdateButton(_soundButton.transform, SingleObject<SoundManager>.instance.CanPlaySound());
	}

	private void OnClickFeedBack()
	{
		ProjectCommonFunction.SendFeedback();
	}

	private void OnClickMusic()
	{
		bool flag = SingleObject<SoundManager>.instance.CanPlayMusic();
		flag = !flag;
		SingleObject<SoundManager>.instance.SetCanPlayMusic(flag);
		ProjectCommonFunction.UpdateButton(_musicButton.transform, flag);
		SingleObject<SoundManager>.instance.PlayWordCuteSound(SoundType.bg, true, !flag);
	}

	private void OnClickSound()
	{
		bool flag = SingleObject<SoundManager>.instance.CanPlaySound();
		flag = !flag;
		SingleObject<SoundManager>.instance.SetCanPlaySound(flag);
		ProjectCommonFunction.UpdateButton(_soundButton.transform, flag);
	}

	private void OnClickHome()
	{
		CloseUI();
		GameMainUICtrl.instance.ToDating(delegate
		{
			SingleObject<UIManager>.instance.OpenUI(UITypeDefine.DatingUI, null, delegate(GameObject obj)
			{
				DatingUICtrl component = obj.GetComponent<DatingUICtrl>();
				component.ShowAnimation();
			});
		});
	}

	private void CloseUI()
	{
		SingleObject<SoundManager>.instance.PlayWordCuteSound(SoundType.menu_close);
		_content.GetComponent<Animation>().ReserverPlay(string.Empty);
		SingleObject<SchudleManger>.instance.Schudle(delegate
		{
			SingleObject<UIManager>.instance.Close(UITypeDefine.GameSetUI);
		}, 0.5f);
	}
}
