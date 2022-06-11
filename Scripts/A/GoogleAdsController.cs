using System;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.Events;

public class GoogleAdsController : SingleObject<GoogleAdsController>
{
	private string ADsIDBanner = string.Empty;

	private string ADsIDInterstitia = "ca-app-pub-81501782874/82673";

	private string ADsIDiVido = "ca-app-pub-815017828763/3945007";

	private string ADsIDBanner_Test = "ca-app-pub-3940256099942/6300978";

	private string ADsIDInterstitia_Test = "ca-app-pub-394025609994/10331";

	private string ADsIDiVido_Test = "ca-app-pub-39402560544/5224317";

	private BannerView bannerView;

	private InterstitialAd interstitial;

	private RewardBasedVideoAd rewardBaseBideoAd;

	public bool currentIntersIsLoadOk;

	public bool currentVidoAdIsLoadOk;

	private UnityAction<bool, string> m_CurrentWatchVidoCallback;

	public void Init()
	{
		InitInterstitial();
		InitRewardBaseBideoAd();
	}

	private void IntiBanner()
	{
		if (Application.isMobilePlatform)
		{
			string empty = string.Empty;
			empty = ((!AppConst.DebugMode) ? ADsIDBanner : ADsIDBanner_Test);
			bannerView = new BannerView(empty, AdSize.SmartBanner, AdPosition.Bottom);
			bannerView.OnAdLoaded += HandleAdLoaded;
			bannerView.OnAdFailedToLoad += HandleAdFailedToLoad;
		}
	}

	private void InitInterstitial()
	{
		string empty = string.Empty;
		empty = ((!AppConst.DebugMode) ? ADsIDInterstitia : ADsIDInterstitia_Test);
		interstitial = new InterstitialAd(empty);
		interstitial.OnAdLoaded += HandleInterstitialLoaded;
		interstitial.OnAdClosed += OnInterstitialClose;
		interstitial.OnAdFailedToLoad += OnInterstitialLoadFail;
	}

	private void InitRewardBaseBideoAd()
	{
		rewardBaseBideoAd = RewardBasedVideoAd.Instance;
		rewardBaseBideoAd.OnAdRewarded += OnVidoAdRewarded;
		rewardBaseBideoAd.OnAdFailedToLoad += OnVidoAdLoadFail;
		rewardBaseBideoAd.OnAdLoaded += OnVidoAdLoadSucess;
		rewardBaseBideoAd.OnAdClosed += OnVidoAdClose;
	}

	public void RequestBanner()
	{
		if (bannerView != null)
		{
			bannerView.LoadAd(createAdRequest());
		}
	}

	public void ShowBanner()
	{
		if (bannerView != null)
		{
			bannerView.Show();
		}
	}

	public void HideBanner()
	{
		if (bannerView != null)
		{
			bannerView.Hide();
		}
	}

	public void RequestInterstitial()
	{
		if (interstitial != null)
		{
			currentIntersIsLoadOk = false;
			interstitial.LoadAd(createAdRequest());
		}
	}

	public void ShowInterstitial()
	{
		if (interstitial != null && interstitial.IsLoaded())
		{
			interstitial.Show();
		}
	}

	public void LoadVidoAd()
	{
		if (rewardBaseBideoAd != null && !currentVidoAdIsLoadOk)
		{
			string empty = string.Empty;
			empty = ((!AppConst.DebugMode) ? ADsIDiVido : ADsIDiVido_Test);
			currentVidoAdIsLoadOk = false;
			rewardBaseBideoAd.LoadAd(createAdRequest(), empty);
		}
	}

	public void PlayVidoAd(UnityAction<bool, string> callback)
	{
		if (rewardBaseBideoAd != null && rewardBaseBideoAd.IsLoaded())
		{
			m_CurrentWatchVidoCallback = callback;
			rewardBaseBideoAd.Show();
			currentVidoAdIsLoadOk = false;
		}
	}

	private AdRequest createAdRequest()
	{
		AdRequest.Builder builder = new AdRequest.Builder();
		return builder.Build();
	}

	public void HandleAdLoaded(object sender, EventArgs args)
	{
		Debug.Log("a banner ad is Load");
	}

	public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
	{
		Debug.Log("a banner ad is Load fail");
		if (Application.internetReachability != 0)
		{
			Debug.Log("a banner ad reLoad");
			SingleObject<SchudleManger>.instance.Schudle(RequestBanner, 3f);
		}
	}

	public void OnInterstitialClose(object sender, EventArgs args)
	{
		RequestInterstitial();
	}

	public void OnInterstitialLoadFail(object sender, AdFailedToLoadEventArgs e)
	{
		Debug.Log("Ad  Admob interstitial  Faild: " + e.Message);
		currentIntersIsLoadOk = false;
	}

	public void HandleInterstitialLoaded(object sender, EventArgs args)
	{
		currentIntersIsLoadOk = true;
		Debug.Log("Ad Admob  interstitial Loaded: ");
	}

	public void OnVidoAdRewarded(object sender, EventArgs args)
	{
		if (m_CurrentWatchVidoCallback != null)
		{
			m_CurrentWatchVidoCallback(true, "Admob");
		}
	}

	public void OnVidoAdClose(object sender, EventArgs args)
	{
		if (m_CurrentWatchVidoCallback != null)
		{
			m_CurrentWatchVidoCallback(false, "Admob");
		}
	}

	public void OnVidoAdLoadSucess(object sender, EventArgs args)
	{
		currentVidoAdIsLoadOk = true;
	}

	public void OnVidoAdLoadFail(object sender, AdFailedToLoadEventArgs e)
	{
	}
}
