using AdColony;
using UnityEngine;
using UnityEngine.Events;

public class AdcController : SingleSysObj<AdcController>
{
	private InterstitialAd Ad;

	private UnityAction<bool, string> callBack;

	private UnityAction<bool> loadCallback;

	private InterstitialAd _AdColony;

	private bool isAdColonyLoaded;

	private bool isBgMusicPlaying;

	private bool setAutoCache;

	private string AdColony_AppID = "appe5309e0673f143e8b1";

	private string AdColony_ZoneID = "vz90831a06167b4099be";

	public void Adcolony_Init(bool autoCache)
	{
		setAutoCache = autoCache;
		Adcolony_Configure();
		Ads.OnRequestInterstitial += Ads_OnRequestInterstitial;
		Ads.OnRequestInterstitialFailed += Ads_OnRequestInterstitialFailed;
		Ads.OnRequestInterstitialFailedWithZone += Ads_OnRequestInterstitialFailedWithZone;
		Ads.OnRewardGranted += Ads_OnRewardGranted;
		Ads.OnOpened += Ads_OnOpened;
		Ads.OnClosed += Ads_OnClosed;
		Ads.OnAudioStarted += Ads_OnAudioStarted;
		Ads.OnAudioStopped += Ads_OnAudioStopped;
	}

	private void Ads_OnRequestInterstitial(InterstitialAd ad_)
	{
		_AdColony = ad_;
		isAdColonyLoaded = true;
		loadCallback.MyInvoke(true);
	}

	private void Ads_OnRequestInterstitialFailedWithZone(string obj)
	{
		Debug.Log("@@@ AdColony.Ads:Ads_OnRequestInterstitialFailedWithZone called: " + obj);
		isAdColonyLoaded = false;
		loadCallback.MyInvoke(false);
	}

	private void Ads_OnRequestInterstitialFailed()
	{
		Debug.Log("@@@ AdColony.Ads:OnRequestInterstitialFailed called");
		isAdColonyLoaded = false;
		if (setAutoCache)
		{
			SingleObject<SchudleManger>.instance.Schudle(delegate
			{
				Adcolony_RequestAd();
			}, 3f);
		}
		loadCallback.MyInvoke(false);
	}

	private void Ads_OnAudioStarted(InterstitialAd obj)
	{
	}

	private void Ads_OnAudioStopped(InterstitialAd obj)
	{
	}

	private void Ads_OnOpened(InterstitialAd obj)
	{
	}

	private void Ads_OnClosed(InterstitialAd obj)
	{
	}

	private void Ads_OnRewardGranted(string zoneId, bool success, string name, int amount)
	{
		if (callBack != null)
		{
			callBack(success, "Adc");
		}
		if (setAutoCache)
		{
			SingleObject<SchudleManger>.instance.Schudle(delegate
			{
				Adcolony_RequestAd();
			}, 3f);
		}
	}

	private void Adcolony_Configure()
	{
		AppOptions appOptions = new AppOptions();
		appOptions.AdOrientation = AdOrientationType.AdColonyOrientationAll;
		string[] zoneIds = new string[1] { AdColony_ZoneID };
		Ads.Configure(AdColony_AppID, appOptions, zoneIds);
	}

	public void Adcolony_RequestAd(UnityAction<bool> callbackS = null)
	{
		if (!isAdColonyLoaded)
		{
			loadCallback = callbackS;
			AdOptions adOptions = new AdOptions();
			adOptions.ShowPrePopup = false;
			adOptions.ShowPostPopup = false;
			Ads.RequestInterstitialAd(AdColony_ZoneID, adOptions);
		}
	}

	public void Adcolony_ShowAd(UnityAction<bool, string> callBack = null)
	{
		this.callBack = callBack;
		if (Adcolony_IsLoaded())
		{
			Ads.ShowAd(_AdColony);
			isAdColonyLoaded = false;
		}
		else
		{
			Adcolony_RequestAd();
		}
	}

	public bool Adcolony_IsLoaded()
	{
		return _AdColony != null && isAdColonyLoaded;
	}
}
