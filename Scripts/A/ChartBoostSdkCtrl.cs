using ChartboostSDK;
using UnityEngine;
using UnityEngine.Events;

public class ChartBoostSdkCtrl : SingleObject<ChartBoostSdkCtrl>
{
	private Chartboost chartboost;

	private string appId = "5bfbb107022cd64a072c1ff0";

	private string signatrue = "381c567fead0709ba7868e28e9f8d0905271fb98";

	private UnityAction<bool, string> _playVideoCallback;

	private bool _videoAdIsLoadOk;

	public bool InterstitialAdIsLoad
	{
		get
		{
			if (!Application.isMobilePlatform)
			{
				return true;
			}
			if (!CheckSDKIsInit())
			{
				return false;
			}
			return Chartboost.hasInterstitial(CBLocation.Default);
		}
	}

	public bool VideoAdIsLoad
	{
		get
		{
			if (!Application.isMobilePlatform)
			{
				return false;
			}
			if (!CheckSDKIsInit())
			{
				return false;
			}
			return _videoAdIsLoadOk;
		}
	}

	public void init()
	{
		if (Application.isMobilePlatform)
		{
			chartboost = Chartboost.CreateWithAppId(appId, signatrue);
			Chartboost.didCompleteRewardedVideo += OnCompleteRewardedVideo;
			Chartboost.didCacheRewardedVideo += OnCacheRewardedVideo;
			Chartboost.didFailToLoadRewardedVideo += OnFailToLoadRewardedVideo;
			Chartboost.didFailToLoadInterstitial += Chartboost_didFailToLoadInterstitial;
			Chartboost.didCacheInterstitial += Chartboost_didCacheInterstitial;
			Util.Log("init chartboost finish");
		}
	}

	private void Chartboost_didCacheInterstitial(CBLocation obj)
	{
		Debug.Log("Ad  CB interstitial  didCache: ");
	}

	private void Chartboost_didFailToLoadInterstitial(CBLocation arg1, CBImpressionError arg2)
	{
		Debug.Log("Ad  CB interstitial  Faild: " + arg2);
	}

	public void LoadInterstitialAd()
	{
		if (CheckSDKIsInit())
		{
			Chartboost.cacheInterstitial(CBLocation.Default);
			Util.Log("load chartBoost interstitialAd");
		}
	}

	public void ShowInterstitialAd()
	{
		if (CheckSDKIsInit())
		{
			Chartboost.showInterstitial(CBLocation.Default);
			Util.Log("Show chartBoost interstitialAd");
		}
	}

	public void LoadVideoAd()
	{
		if (CheckSDKIsInit())
		{
			_videoAdIsLoadOk = false;
			Chartboost.cacheRewardedVideo(CBLocation.Default);
			Util.Log("load chartBoost videoAd");
		}
	}

	public void ShowVideoAd(UnityAction<bool, string> playCallback = null)
	{
		if (CheckSDKIsInit())
		{
			_playVideoCallback = playCallback;
			_videoAdIsLoadOk = false;
			Chartboost.showRewardedVideo(CBLocation.Default);
			Util.Log("show chartBoost videoAd");
		}
	}

	private void OnCompleteRewardedVideo(CBLocation location, int reward)
	{
		Util.Log("complete Play RewardedVideo");
		if (_playVideoCallback != null)
		{
			_playVideoCallback(true, "CB");
		}
	}

	private bool CheckSDKIsInit()
	{
		if (Chartboost.isInitialized())
		{
			return true;
		}
		return false;
	}

	private void OnCacheRewardedVideo(CBLocation location)
	{
		Util.Log("have a video Load success");
		_videoAdIsLoadOk = true;
	}

	private void OnFailToLoadRewardedVideo(CBLocation location, CBImpressionError error)
	{
		Util.LogWarning("have a video Load fail ---> " + error);
	}
}
