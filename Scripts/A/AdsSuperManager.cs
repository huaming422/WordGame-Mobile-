using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AdsSuperManager : SingleObject<AdsSuperManager>
{
	private bool canChartBoost = true;

	private bool canGoogle = true;

	private bool canAdconoly = true;

	private bool canAmazon;

	private int _nowShowVideoAdTimes;

	private List<int> showPro = new List<int> { 30, 70, 100 };

	private bool usedTimeCut;

	private float timeLimie = 24f;

	private int wheelVideo;

	private int wheelIns
	{
		get
		{
			return PlayerPrefs.GetInt("wheelInsKey", 0);
		}
		set
		{
			PlayerPrefs.SetInt("wheelInsKey", value);
		}
	}

	private bool MoreThanSetHour()
	{
		if (!usedTimeCut)
		{
			return true;
		}
		if (PlayerDataManager.InstallForNow.TotalHours < (double)timeLimie)
		{
			return false;
		}
		return true;
	}

	public void Init()
	{
		if (canChartBoost)
		{
			SingleObject<ChartBoostSdkCtrl>.instance.init();
		}
		if (canAmazon)
		{
			SingleSysObj<AmzonAdManager>.instance.OutInit();
		}
		if (canAdconoly)
		{
			SingleSysObj<AdcController>.instance.Adcolony_Init(true);
		}
		if (canGoogle)
		{
			SingleObject<GoogleAdsController>.instance.Init();
		}
	}

	public bool InterAdIsLoad()
	{
		if (AppConst.Channel == GameChannel.Amazon)
		{
			return SingleObject<ChartBoostSdkCtrl>.instance.InterstitialAdIsLoad && canChartBoost;
		}
		if (AppConst.Channel == GameChannel.Google)
		{
			if (!MoreThanSetHour())
			{
				return SingleObject<GoogleAdsController>.instance.currentIntersIsLoadOk && canGoogle;
			}
			return (SingleObject<GoogleAdsController>.instance.currentIntersIsLoadOk && canGoogle) || (SingleObject<ChartBoostSdkCtrl>.instance.InterstitialAdIsLoad && canChartBoost);
		}
		return false;
	}

	public void LoadInterAd()
	{
		if (AppConst.IsCloseAds || PlayerData.i.playerBuyRemoveAd)
		{
			return;
		}
		if (AppConst.Channel == GameChannel.Amazon)
		{
			if (!SingleObject<ChartBoostSdkCtrl>.instance.InterstitialAdIsLoad && canChartBoost)
			{
				SingleObject<ChartBoostSdkCtrl>.instance.LoadInterstitialAd();
			}
		}
		else if (AppConst.Channel == GameChannel.Google)
		{
			if (!SingleObject<ChartBoostSdkCtrl>.instance.InterstitialAdIsLoad && canChartBoost)
			{
				SingleObject<ChartBoostSdkCtrl>.instance.LoadInterstitialAd();
			}
			if (!SingleObject<GoogleAdsController>.instance.currentIntersIsLoadOk && canGoogle)
			{
				SingleObject<GoogleAdsController>.instance.RequestInterstitial();
			}
		}
	}

	public void ShowInterAd()
	{
		if (AppConst.IsCloseAds || PlayerData.i.playerBuyRemoveAd)
		{
			return;
		}
		if (AppConst.Channel == GameChannel.Amazon)
		{
			if (SingleObject<ChartBoostSdkCtrl>.instance.InterstitialAdIsLoad && canChartBoost)
			{
				SingleObject<ChartBoostSdkCtrl>.instance.ShowInterstitialAd();
			}
		}
		else
		{
			if (AppConst.Channel != GameChannel.Google)
			{
				return;
			}
			bool currentIntersIsLoadOk = SingleObject<GoogleAdsController>.instance.currentIntersIsLoadOk;
			bool interstitialAdIsLoad = SingleObject<ChartBoostSdkCtrl>.instance.InterstitialAdIsLoad;
			if (!MoreThanSetHour())
			{
				if (!SingleObject<GoogleAdsController>.instance.currentIntersIsLoadOk)
				{
					if (SingleObject<ChartBoostSdkCtrl>.instance.InterstitialAdIsLoad && canChartBoost)
					{
						SingleObject<ChartBoostSdkCtrl>.instance.ShowInterstitialAd();
					}
				}
				else if (canGoogle)
				{
					SingleObject<GoogleAdsController>.instance.ShowInterstitial();
				}
			}
			else if ((wheelIns++ % 2 == 0 && SingleObject<ChartBoostSdkCtrl>.instance.InterstitialAdIsLoad) || !SingleObject<GoogleAdsController>.instance.currentIntersIsLoadOk)
			{
				if (canChartBoost)
				{
					SingleObject<ChartBoostSdkCtrl>.instance.ShowInterstitialAd();
				}
			}
			else if (canGoogle)
			{
				SingleObject<GoogleAdsController>.instance.ShowInterstitial();
			}
		}
	}

	public bool VideoAdIsLoadOk()
	{
		if (AppConst.Channel == GameChannel.Amazon)
		{
			return (SingleObject<ChartBoostSdkCtrl>.instance.VideoAdIsLoad && canChartBoost) || (SingleSysObj<AdcController>.instance.Adcolony_IsLoaded() && canAdconoly);
		}
		if (AppConst.Channel == GameChannel.Google)
		{
			bool currentVidoAdIsLoadOk = SingleObject<GoogleAdsController>.instance.currentVidoAdIsLoadOk;
			bool flag = SingleSysObj<AdcController>.instance.Adcolony_IsLoaded();
			bool videoAdIsLoad = SingleObject<ChartBoostSdkCtrl>.instance.VideoAdIsLoad;
			return currentVidoAdIsLoadOk || flag || videoAdIsLoad;
		}
		return false;
	}

	public void LoadVideoAD(UnityAction callback = null)
	{
		if (AppConst.IsCloseAds)
		{
			return;
		}
		if (AppConst.Channel == GameChannel.Amazon)
		{
			if (!SingleObject<ChartBoostSdkCtrl>.instance.VideoAdIsLoad && canChartBoost)
			{
				SingleObject<ChartBoostSdkCtrl>.instance.LoadVideoAd();
			}
			if (!SingleSysObj<AdcController>.instance.Adcolony_IsLoaded() && canAdconoly)
			{
				SingleSysObj<AdcController>.instance.Adcolony_RequestAd();
			}
		}
		else if (AppConst.Channel == GameChannel.Google)
		{
			if (canAdconoly)
			{
				SingleSysObj<AdcController>.instance.Adcolony_RequestAd();
			}
			if (canChartBoost)
			{
				SingleObject<ChartBoostSdkCtrl>.instance.LoadVideoAd();
			}
			if (canGoogle)
			{
				SingleObject<GoogleAdsController>.instance.LoadVidoAd();
			}
		}
		if (callback != null)
		{
			callback();
		}
	}

	public void PlayVideoAD(UnityAction<bool, string> callBack)
	{
		if (AppConst.IsCloseAds || AppConst.Channel == GameChannel.Amazon || AppConst.Channel != GameChannel.Google)
		{
			return;
		}
		UnityAction<bool, string> unityAction = delegate(bool result, string from)
		{
			SingleObject<SchudleManger>.instance.Schudle(delegate
			{
				callBack(result, from);
			}, 0.1f);
		};
		bool currentVidoAdIsLoadOk = SingleObject<GoogleAdsController>.instance.currentVidoAdIsLoadOk;
		bool flag = SingleSysObj<AdcController>.instance.Adcolony_IsLoaded();
		bool videoAdIsLoad = SingleObject<ChartBoostSdkCtrl>.instance.VideoAdIsLoad;
		int num = Random.Range(0, 100);
		Debug.Log("curr Show Ads?   hadAdmob : " + currentVidoAdIsLoadOk + "  hadAdc: " + flag + "  hadCB: " + videoAdIsLoad + "  randomSize " + num);
		if ((num < showPro[0] && currentVidoAdIsLoadOk) || (!videoAdIsLoad && !flag))
		{
			Debug.Log("Show Ads  Admob");
			SingleObject<GoogleAdsController>.instance.PlayVidoAd(unityAction);
		}
		else if ((num < showPro[1] && videoAdIsLoad) || (!currentVidoAdIsLoadOk && !flag))
		{
			Debug.Log("Show Ads  ChartBoostSdkCtrl");
			SingleObject<ChartBoostSdkCtrl>.instance.ShowVideoAd(unityAction);
		}
		else if (num <= showPro[2] && flag)
		{
			Debug.Log("Show Ads  adc");
			SingleSysObj<AdcController>.instance.Adcolony_ShowAd(unityAction);
		}
	}
}
