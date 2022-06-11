using System.Collections;
using UnityEngine;

namespace AdColony
{
	public class AdsAndroid : IAds
	{
		private AndroidJavaClass _plugin;

		private AndroidJavaClass _pluginWrapper;

		public AdsAndroid(string managerName)
		{
			_plugin = new AndroidJavaClass("com.adcolony.sdk.AdColony");
			_pluginWrapper = new AndroidJavaClass("com.adcolony.unityplugin.UnityADCAds");
			_pluginWrapper.CallStatic("setManagerName", managerName);
		}

		public void Configure(string appId, AppOptions appOptions, params string[] zoneIds)
		{
			Hashtable hashtable = new Hashtable();
			hashtable["app_id"] = appId;
			hashtable["zone_ids"] = new ArrayList(zoneIds);
			if (appOptions == null)
			{
				appOptions = new AppOptions();
			}
			appOptions.SetOption("plugin_version", "3.3.5");
			hashtable["app_options"] = appOptions.ToHashtable();
			string text = AdColonyJson.Encode(hashtable);
			_pluginWrapper.CallStatic("configure", text);
		}

		public string GetSDKVersion()
		{
			return _plugin.CallStatic<string>("getSDKVersion", new object[0]);
		}

		public void RequestInterstitialAd(string zoneId, AdOptions adOptions)
		{
			Hashtable hashtable = new Hashtable();
			hashtable["zone_id"] = zoneId;
			if (adOptions != null)
			{
				hashtable["ad_options"] = adOptions.ToHashtable();
			}
			string text = AdColonyJson.Encode(hashtable);
			_pluginWrapper.CallStatic("requestInterstitialAd", text);
		}

		public Zone GetZone(string zoneId)
		{
			string json = _pluginWrapper.CallStatic<string>("getZone", new object[1] { zoneId });
			Hashtable values = AdColonyJson.Decode(json) as Hashtable;
			return new Zone(values);
		}

		public string GetUserID()
		{
			AppOptions appOptions = GetAppOptions();
			if (appOptions != null)
			{
				return appOptions.UserId;
			}
			return null;
		}

		public void SetAppOptions(AppOptions appOptions)
		{
			string text = null;
			if (appOptions != null)
			{
				text = appOptions.ToJsonString();
			}
			_pluginWrapper.CallStatic("setAppOptions", text);
		}

		public AppOptions GetAppOptions()
		{
			string text = _pluginWrapper.CallStatic<string>("getAppOptions", new object[0]);
			Hashtable values = new Hashtable();
			if (text != null)
			{
				values = AdColonyJson.Decode(text) as Hashtable;
			}
			return new AppOptions(values);
		}

		public void SendCustomMessage(string type, string content)
		{
			_pluginWrapper.CallStatic("sendCustomMessage", type, content);
		}

		public void LogInAppPurchase(string transactionId, string productId, int purchasePriceMicro, string currencyCode)
		{
			_plugin.CallStatic<bool>("notifyIAPComplete", new object[4]
			{
				productId,
				transactionId,
				currencyCode,
				(double)purchasePriceMicro / 1000000.0
			});
		}

		public void ShowAd(InterstitialAd ad)
		{
			_pluginWrapper.CallStatic("showAd", ad.Id);
		}

		public void CancelAd(InterstitialAd ad)
		{
			_pluginWrapper.CallStatic("cancelAd", ad.Id);
		}

		public void DestroyAd(string id)
		{
			_pluginWrapper.CallStatic("destroyAd", id);
		}

		public void RegisterForCustomMessage(string type)
		{
			_pluginWrapper.CallStatic("registerForCustomMessage", type);
		}

		public void UnregisterForCustomMessage(string type)
		{
			_pluginWrapper.CallStatic("unregisterForCustomMessage", type);
		}
	}
}
