using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DatingUICtrl : MonoBehaviour
{
	private Transform _content;

	private DatingLevelCtrl _levelCtrl;

	private int _nowOpenLevel;

	private Transform _otherPart;

	private Button _setButton;

	private Button _signButton;

	private Button _addCoinButton;

	private Text _coinText;

	private void Start()
	{
		_content = base.transform.Find("Content");
		_levelCtrl = _content.Find("Level").GetComponent<DatingLevelCtrl>();
		_nowOpenLevel = PlayerData.i.openLevel;
		_otherPart = _content.Find("OtherPart");
		_setButton = _otherPart.Find("Set").GetComponent<Button>();
		_signButton = _otherPart.Find("SignIn").GetComponent<Button>();
		_addCoinButton = _otherPart.Find("Coin").GetComponent<Button>();
		_coinText = _addCoinButton.transform.Find("Text").GetComponent<Text>();
		_levelCtrl.onButtonClick = OnSelectLevel;
		_levelCtrl.onButtonChange = OnButtonChange;
		SingleObject<SchudleManger>.instance.Schudle(DelayInitUI, 0.1f);
		InitUI();
		SingleObject<GamePlayPosManager>.instance.onAnglePress.AddListener(OnQuitGame);
		SingleObject<MessageManger>.instance.AddEvent(100, OnCoinCountChange);
		InitAd();
		CheckOpenRateUI();
		SingleObject<CacheDataManager>.instance.RemoveBykey("datakey_12002");
	}

	private void OnDestroy()
	{
		SingleObject<GamePlayPosManager>.instance.onAnglePress.RemoveAllListeners();
		SingleObject<MessageManger>.instance.RemoveEvent(100, OnCoinCountChange);
	}

	public void ShowAnimation()
	{
		if (_content == null)
		{
			_content = base.transform.Find("Content");
		}
		CanvasGroup canvasGroup = _content.GetComponent<CanvasGroup>();
		canvasGroup.alpha = 0f;
		SingleObject<SchudleManger>.instance.Schudle(delegate
		{
			canvasGroup.DOFade(1f, 0.3f);
		}, 0.2f);
	}

	private void InitAd()
	{
		WordCuteData.intoDatingTimes++;
		if (WordCuteData.intoDatingTimes > 1)
		{
			SingleObject<AdsSuperManager>.instance.ShowInterAd();
			SingleObject<SchudleManger>.instance.Schudle(delegate
			{
				SingleObject<AdsSuperManager>.instance.LoadInterAd();
			}, 0.5f);
		}
	}

	private void OnNotifiShowInterAd(int id, object mes)
	{
		SingleObject<AdsSuperManager>.instance.ShowInterAd();
		SingleObject<SchudleManger>.instance.Schudle(delegate
		{
			SingleObject<AdsSuperManager>.instance.LoadInterAd();
		}, 0.5f);
	}

	private void InitUI()
	{
		_setButton.onClick.AddListener(OnClickSet);
		_addCoinButton.onClick.AddListener(OnClickAddCoin);
		_coinText.text = ProjectCommonFunction.GetMonoyString(PlayerData.i.coinCount);
		SingleObject<CacheDataManager>.instance.Cache("datakey_12001", _coinText.transform.position, 86400f);
	}

	private void DelayInitUI()
	{
		int openLevel = PlayerData.i.openLevel;
		_levelCtrl.JumpToNowPos(openLevel, false);
	}

	private void OnSelectLevel(LevelButtonCtrl buttonCtrl)
	{
		SingleObject<SoundManager>.instance.PlayWordCuteSound(SoundType.click);
		int level = buttonCtrl.level;
		if (AppConst.IsOpenLevel)
		{
			WordCuteTools.OpenGameMainLevel(level);
		}
		else if (level <= _nowOpenLevel)
		{
			WordCuteTools.OpenGameMainLevel(level);
		}
	}

	private void OnButtonChange(LevelButtonCtrl buttonCtrl)
	{
		if (buttonCtrl.level == _nowOpenLevel)
		{
			buttonCtrl.SetIsPass();
		}
	}

	private void OnClickSet()
	{
		SingleObject<UIManager>.instance.OpenUI(UITypeDefine.GameSetUI);
	}

	private void OnClickAddCoin()
	{
		GameChargeUICtrl.Show(delegate
		{
			_coinText.text = ProjectCommonFunction.GetMonoyString(PlayerData.i.coinCount);
		});
	}

	private void OnCoinCountChange(int id, object mes)
	{
		_coinText.text = ProjectCommonFunction.GetMonoyString(PlayerData.i.coinCount);
	}

	private void OnQuitGame()
	{
		QuitUICtrl.OpenUI(1, delegate(bool isOk)
		{
			if (isOk)
			{
				Application.Quit();
			}
		});
	}

	private void CheckOpenRateUI()
	{
		if (PlayerData.i.rateGameSate == 0 && PlayerDataManager.InstallForNow.TotalHours > 72.0 && PlayerData.i.openLevel > 60)
		{
			SingleObject<UIManager>.instance.OpenUI(UITypeDefine.GameRateUI);
		}
	}
}
