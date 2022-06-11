using UnityEngine;

public class AmzonAdManager : SingleSysObj<AmzonAdManager>
{
	private bool _isTest = true;

	private AndroidJavaObject _adsManager;

	private string _appKey = "6dd64c6fcda740ff970aaa2caddee6e3";

	private bool _interAdisLoad;

	public bool interAdisLoad
	{
		get
		{
			return _interAdisLoad;
		}
	}

	protected override void Init()
	{
	}

	public void OutInit()
	{
	}

	public void LoadInterAd()
	{
		if (_adsManager != null)
		{
			_interAdisLoad = false;
			_adsManager.Call("LoadInterAd");
		}
	}

	public bool InterAdIsLoad()
	{
		if (_adsManager == null)
		{
			return false;
		}
		return _adsManager.Call<bool>("InterAdIsLoad", new object[0]);
	}

	public void ShowInterAd()
	{
		if (_adsManager != null)
		{
			_interAdisLoad = false;
			if (InterAdIsLoad())
			{
				_adsManager.Call("ShowInterAd");
			}
		}
	}

	public void LoadBanner()
	{
		if (_adsManager != null)
		{
			_adsManager.Call("LoadBannerAd");
		}
	}

	public bool BannerIsLoadOk()
	{
		if (_adsManager == null)
		{
			return false;
		}
		return _adsManager.Call<bool>("BannerAdIsLoad", new object[0]);
	}

	public void ShowBanner()
	{
		if (_adsManager != null)
		{
			_adsManager.Call("ShowBannerAd");
		}
	}

	public void HiddenBanner()
	{
		if (_adsManager != null)
		{
			_adsManager.Call("HiddenBanner");
		}
	}

	private void OnReceiveLoadMessage(int code, string data)
	{
		if (code == 1)
		{
			_interAdisLoad = true;
		}
		if (code == 0)
		{
			Util.LogWarning("load AmazonInterAd error:" + data);
		}
		if (code == 2)
		{
			Util.Log("AmazonInterAd closed");
		}
		if (code == 11)
		{
			MessageManger.SendMessage(6);
		}
	}
}
