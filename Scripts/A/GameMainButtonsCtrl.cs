using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameMainButtonsCtrl : MonoBehaviour
{
	private Button _setButton;

	private Button _randChangeButton;

	private Button _bounsButton;

	private Button _tipButton;

	private Button _tipHandButton;

	private Button _addCoinButton;

	private Text _levelText;

	private Text _coinText;

	private Text _bounsText;

	private RectTransform _selfRect;

	private int _showBannerRectHeight = 1162;

	private void Start()
	{
		Debug.Log(base.gameObject.name);
		_setButton = base.transform.Find("Set").GetComponent<Button>();
		_randChangeButton = base.transform.Find("ChangePos").GetComponent<Button>();
		_bounsButton = base.transform.Find("Bouns").GetComponent<Button>();
		_tipButton = base.transform.Find("Tip").GetComponent<Button>();
		_tipHandButton = base.transform.Find("HandTip").GetComponent<Button>();
		_addCoinButton = base.transform.Find("Coin").GetComponent<Button>();
		_levelText = base.transform.Find("Level").GetComponent<Text>();
		_coinText = _addCoinButton.transform.Find("Text").GetComponent<Text>();
		_bounsText = _bounsButton.transform.Find("Count").GetComponent<Text>();
		_levelText.gameObject.SetActive(false);
		_coinText.gameObject.SetActive(false);
		_bounsText.gameObject.SetActive(false);
		_setButton.onClick.AddListener(OnClickSetButton);
		_randChangeButton.onClick.AddListener(OnClickChangeButton);
		_bounsButton.onClick.AddListener(OnClickBounsButton);
		_tipButton.onClick.AddListener(OnClickTipButton);
		_tipHandButton.onClick.AddListener(OnClickTipHandButton);
		_addCoinButton.onClick.AddListener(OnClickAddCoinButton);
		_selfRect = base.transform as RectTransform;
		InitAd();
		SingleObject<MessageManger>.instance.AddEvent(6, OnBannerAdLoadSucess);
	}

	private void OnDestroy()
	{
		SingleObject<MessageManger>.instance.RemoveEvent(6, OnBannerAdLoadSucess);
	}

	private void InitAd()
	{
		SingleObject<AdsSuperManager>.instance.LoadVideoAD();
	}

	public void InitUI()
	{
		_levelText.text = WordCuteData.nowLevel.ToString();
		_coinText.text = ProjectCommonFunction.GetMonoyString(PlayerData.i.coinCount);
		UpdateBounsCount();
		PlayerTextShowAnimation();
	}

	private void OnBannerAdLoadSucess(int id, object mes)
	{
		Vector2 size = _selfRect.rect.size;
		size.Set(size.x, _showBannerRectHeight);
		_selfRect.DOSizeDelta(size, WordConfig.duration);
	}

	private void PlayerTextShowAnimation()
	{
		_levelText.gameObject.SetActive(true);
		CanvasGroup component = _levelText.GetComponent<CanvasGroup>();
		component.alpha = 0f;
		component.DOFade(1f, WordConfig.duration);
		_coinText.gameObject.SetActive(true);
		_bounsText.gameObject.SetActive(true);
	}

	public void UpdateCoinText(bool isAnimation = false)
	{
		if (!isAnimation)
		{
			_coinText.text = ProjectCommonFunction.GetMonoyString(PlayerData.i.coinCount);
		}
	}

	private void OnClickSetButton()
	{
		SingleObject<UIManager>.instance.OpenUI(UITypeDefine.GameSetUI, null, delegate(GameObject obj)
		{
			SetUICtrl component = obj.GetComponent<SetUICtrl>();
			component.showType = 2;
		});
	}

	private void OnClickChangeButton()
	{
		AplhabetSelectCtrl.instance.ReDoItemPos();
	}

	private void OnClickEarnButton()
	{
		SingleObject<UIManager>.instance.OpenUI(UITypeDefine.EarnCoinUI);
	}

	private void OnClickBounsButton()
	{
		SingleObject<UIManager>.instance.OpenUI(UITypeDefine.GameBounsUI);
	}

	private void OnClickTipButton()
	{
		if (PlayerData.i.coinCount < WordConfig.tipCoinCount)
		{
			GameChargeUICtrl.Show(delegate
			{
				MessageManger.SendMessage(100);
			});
		}
		else if (AplhabetTableCtrl.instance.RandomTip())
		{
			PlayerDataManager.ChangeCoin(-WordConfig.tipCoinCount);
		}
	}

	private void OnClickTipHandButton()
	{
		if (WordCuteData.gameState != GameSate.Playing)
		{
			return;
		}
		if (PlayerData.i.coinCount < WordConfig.diyTipCoinCount)
		{
			GameChargeUICtrl.Show(delegate
			{
				MessageManger.SendMessage(100);
			});
		}
		else
		{
			SingleObject<UIManager>.instance.OpenUI(UITypeDefine.UseHandTipUI);
			WordCuteData.gameState = GameSate.WaitUseTipHand;
		}
	}

	private void OnClickAddCoinButton()
	{
		GameChargeUICtrl.Show(delegate
		{
			MessageManger.SendMessage(100);
		});
	}

	public void FlyBounsWord(string word, UnityAction callback = null)
	{
		Vector3 showTextPos = AplhabetSelectCtrl.instance.GetShowTextPos();
		Vector3 position = _bounsButton.transform.position;
		AplhabetTableCtrl.instance.FlyBounsWord(word, showTextPos, position, callback);
	}

	public Vector3 GetCoinPos()
	{
		return _coinText.transform.position;
	}

	public void UpdateBounsCount()
	{
		_bounsText.text = WordCuteTools.GetSelectBounsWord().Length.ToString();
	}

	public void UpdateBounsCountByAnimaton()
	{
		_bounsButton.transform.DOShakeScale(WordConfig.duration, 0.3f);
		_bounsText.text = WordCuteTools.GetSelectBounsWord().Length.ToString();
	}
}
