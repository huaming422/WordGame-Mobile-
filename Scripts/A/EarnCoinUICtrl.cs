using UnityEngine;
using UnityEngine.UI;

public class EarnCoinUICtrl : MonoBehaviour
{
	private Transform _content;

	private Button _watchVideo;

	private Button _surveyWall;

	private Button _close;

	private bool _lastAdLoadState;

	private bool _lastSurveyState;

	private void Start()
	{
		_content = base.transform.Find("Content");
		_watchVideo = _content.Find("Video").GetComponent<Button>();
		_surveyWall = _content.Find("ServayWall").GetComponent<Button>();
		_close = _content.Find("Close").GetComponent<Button>();
		_watchVideo.onClick.AddListener(OnClikWatchVideo);
		_surveyWall.onClick.AddListener(OnClickSurveyWall);
		_close.onClick.AddListener(CloseUI);
		_content.GetComponent<Animation>().OrderPlay(string.Empty);
		SingleObject<SoundManager>.instance.PlayWordCuteSound(SoundType.enu_open);
		InitUI();
	}

	private void InitUI()
	{
		_lastAdLoadState = SingleObject<AdsSuperManager>.instance.VideoAdIsLoadOk();
		_watchVideo.interactable = _lastAdLoadState;
		if (!_lastAdLoadState)
		{
			SingleObject<AdsSuperManager>.instance.LoadVideoAD();
		}
	}

	private void Update()
	{
		if (_lastAdLoadState != SingleObject<AdsSuperManager>.instance.VideoAdIsLoadOk())
		{
			_lastAdLoadState = SingleObject<AdsSuperManager>.instance.VideoAdIsLoadOk();
			_watchVideo.interactable = _lastAdLoadState;
		}
	}

	private void OnClikWatchVideo()
	{
	}

	private void OnClickSurveyWall()
	{
	}

	private void CloseUI()
	{
		SingleObject<SoundManager>.instance.PlayWordCuteSound(SoundType.menu_close);
		_content.GetComponent<Animation>().ReserverPlay(string.Empty);
		SingleObject<UIManager>.instance.DelayColse(UITypeDefine.EarnCoinUI, 0.5f);
	}
}
