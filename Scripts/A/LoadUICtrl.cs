using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoadUICtrl : MonoBehaviour
{
	public static LoadUICtrl instance;

	private Transform _content;

	private Slider m_Slider;

	private Text m_ProgressText;

	private Button _playButton;

	private float m_StartTime;

	private float m_StartValue;

	private float m_EndValue = 1f;

	private bool m_IsCheckWaithThing = true;

	private bool m_IsUpdate;

	private bool m_WaitThingIsOk = true;

	public float maxWaitTime = 30f;

	public Func<bool> waitThing;

	public Action onFinish;

	private void Start()
	{
		instance = this;
		_content = base.transform.Find("Content");
		m_Slider = base.transform.Find("Content/Progress").GetComponent<Slider>();
		m_ProgressText = base.transform.Find("Content/ProgressText/Text").GetComponent<Text>();
		_playButton = _content.Find("Play").GetComponent<Button>();
		_playButton.onClick.AddListener(OnClickPlayButton);
		m_StartTime = Time.time;
		if (waitThing == null)
		{
			m_IsCheckWaithThing = false;
		}
	}

	private void Update()
	{
		if (m_IsUpdate)
		{
			m_Slider.value = Mathf.Lerp(m_StartValue, m_EndValue, (Time.time - m_StartTime) / maxWaitTime);
			m_ProgressText.text = "Loading:" + Mathf.CeilToInt(m_Slider.value * 100f) + "%";
			if (Time.time - m_StartTime > maxWaitTime)
			{
				m_IsUpdate = false;
				ShowPlay();
			}
			if (m_IsCheckWaithThing && waitThing() && m_WaitThingIsOk)
			{
				m_IsCheckWaithThing = false;
				m_StartValue = m_Slider.value;
				m_StartTime = Time.time;
				maxWaitTime = 1f;
			}
		}
	}

	private void ShowPlay()
	{
		SingleObject<SoundManager>.instance.PlayWordCuteSound(SoundType.bg, true);
		m_Slider.gameObject.SetActive(false);
		m_ProgressText.gameObject.SetActive(false);
		_playButton.gameObject.SetActive(true);
		_playButton.transform.DOScale(1.1f, 1f).SetLoops(10000, LoopType.Yoyo);
	}

	private void DoLoadFinish()
	{
		_content.GetComponent<Animation>().ReserverPlay(string.Empty);
		SingleObject<SchudleManger>.instance.Schudle(delegate
		{
			SingleObject<UIManager>.instance.Close(UITypeDefine.LoadingUI);
			MessageManger.SendMessage(4);
			if (onFinish != null)
			{
				onFinish();
			}
		}, 0.5f);
	}

	private static void CheckGoodsIsPurch()
	{
		if (PlayerData.i.playerBuyRemoveAd)
		{
			return;
		}
		SingleObject<UnityInAppPurchManager>.instance.CheckGoodIsPurch(5.ToString(), delegate(bool isPurch)
		{
			if (isPurch)
			{
				PlayerData.i.playerBuyRemoveAd = true;
				PlayerData.Save();
			}
		});
	}

	public static void ShowLoading(float maxTime, Func<bool> waitThing, Action callback, UnityAction openUICallback = null)
	{
		SingleObject<UIManager>.instance.OpenUI(UITypeDefine.LoadingUI, null, delegate(GameObject obj)
		{
			LoadUICtrl loadCtrl = obj.GetComponent<LoadUICtrl>();
			loadCtrl.maxWaitTime = maxTime;
			loadCtrl.waitThing = waitThing;
			loadCtrl.onFinish = callback;
			SingleObject<SchudleManger>.instance.Schudle(delegate
			{
				openUICallback.MyInvoke();
				CheckGoodsIsPurch();
				loadCtrl.LoadAd();
				loadCtrl.m_IsUpdate = true;
			}, 0.2f);
		});
	}

	private void LoadAd()
	{
		SingleObject<AdsSuperManager>.instance.LoadInterAd();
		if (AppConst.DebugMode)
		{
			m_WaitThingIsOk = true;
		}
	}

	private void OnClickPlayButton()
	{
		SingleObject<SoundManager>.instance.PlayWordCuteSound(SoundType.click);
		SingleObject<UIManager>.instance.OpenUI(UITypeDefine.DatingUI, null, delegate(GameObject obj)
		{
			obj.transform.SetAsFirstSibling();
			DoLoadFinish();
		});
	}
}
