using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdColony
{
	public class Ads : MonoBehaviour
	{
		private Dictionary<string, InterstitialAd> _ads = new Dictionary<string, InterstitialAd>();

		private static Ads _sharedGameObject;

		private static bool _initialized;

		private IAds _sharedInstance;

		private IEventTracker _eventTracker;

		private object _updateOnMainThreadActionsLock = new object();

		private readonly Queue<Action> _updateOnMainThreadActions = new Queue<Action>();

		public static Ads SharedGameObject
		{
			get
			{
				if (!_sharedGameObject)
				{
					_sharedGameObject = (Ads)UnityEngine.Object.FindObjectOfType(typeof(Ads));
				}
				if (!_sharedGameObject)
				{
					GameObject gameObject = new GameObject();
					_sharedGameObject = gameObject.AddComponent<Ads>();
					gameObject.name = Constants.AdsManagerName;
					UnityEngine.Object.DontDestroyOnLoad(gameObject);
					if (_sharedGameObject._sharedInstance != null)
					{
						Debug.LogWarning(Constants.AdsMessageAlreadyInitialized);
					}
					else
					{
						_sharedGameObject._sharedInstance = null;
						_sharedGameObject._sharedInstance = new AdsAndroid(gameObject.name);
						_sharedGameObject._eventTracker = new EventTrackerAndroid();
					}
				}
				return _sharedGameObject;
			}
		}

		private static IAds SharedInstance
		{
			get
			{
				IAds ads = null;
				Ads sharedGameObject = SharedGameObject;
				if (sharedGameObject != null)
				{
					ads = sharedGameObject._sharedInstance;
				}
				if (ads == null)
				{
				}
				return ads;
			}
		}

		public static event Action<List<Zone>> OnConfigurationCompleted;

		public static event Action<InterstitialAd> OnRequestInterstitial;

		public static event Action OnRequestInterstitialFailed;

		public static event Action<string> OnRequestInterstitialFailedWithZone;

		public static event Action<string, string> OnCustomMessageReceived;

		public static event Action<InterstitialAd> OnOpened;

		public static event Action<InterstitialAd> OnClosed;

		public static event Action<InterstitialAd> OnExpiring;

		public static event Action<InterstitialAd> OnAudioStarted;

		public static event Action<InterstitialAd> OnAudioStopped;

		public static event Action<InterstitialAd> OnLeftApplication;

		public static event Action<InterstitialAd> OnClicked;

		public static event Action<InterstitialAd, string, AdsIAPEngagementType> OnIAPOpportunity;

		public static event Action<string, bool, string, int> OnRewardGranted;

		public static void Configure(string appId, AppOptions options, params string[] zoneIds)
		{
			if (SharedInstance == null)
			{
				Debug.LogWarning(Constants.AdsMessageSDKUnavailable);
				return;
			}
			SharedInstance.Configure(appId, options, zoneIds);
			_initialized = true;
		}

		public static void RequestInterstitialAd(string zoneId, AdOptions adOptions)
		{
			if (IsInitialized())
			{
				SharedInstance.RequestInterstitialAd(zoneId, adOptions);
			}
		}

		public static void ShowAd(InterstitialAd ad)
		{
			if (IsInitialized())
			{
				if (ad != null)
				{
					SharedInstance.ShowAd(ad);
				}
				else
				{
					Debug.LogError(Constants.AdsMessageErrorNullAd);
				}
			}
		}

		public static void SetAppOptions(AppOptions options)
		{
			if (IsInitialized())
			{
				SharedInstance.SetAppOptions(options);
			}
		}

		public static AppOptions GetAppOptions()
		{
			if (IsInitialized())
			{
				return SharedInstance.GetAppOptions();
			}
			return null;
		}

		public static string GetSDKVersion()
		{
			if (IsInitialized())
			{
				return SharedInstance.GetSDKVersion();
			}
			return null;
		}

		public static Zone GetZone(string zoneId)
		{
			if (IsInitialized())
			{
				return SharedInstance.GetZone(zoneId);
			}
			return null;
		}

		public static string GetUserID()
		{
			if (IsInitialized())
			{
				return SharedInstance.GetUserID();
			}
			return null;
		}

		public static void SendCustomMessage(string type, string content)
		{
			if (IsInitialized())
			{
				SharedInstance.SendCustomMessage(type, content);
			}
		}

		public static void LogInAppPurchase(string transactionId, string productId, int purchasePriceMicro, string currencyCode)
		{
			if (IsInitialized())
			{
				SharedInstance.LogInAppPurchase(transactionId, productId, purchasePriceMicro, currencyCode);
			}
		}

		public static void CancelAd(InterstitialAd ad)
		{
			if (IsInitialized())
			{
				if (ad != null)
				{
					SharedInstance.CancelAd(ad);
				}
				else
				{
					Debug.LogError(Constants.AdsMessageErrorNullAd);
				}
			}
		}

		public static IEventTracker GetEventTracker()
		{
			IEventTracker eventTracker = null;
			if (SharedGameObject != null)
			{
				eventTracker = SharedGameObject._eventTracker;
			}
			if (eventTracker == null)
			{
				Debug.LogError(Constants.AdsMessageErrorInvalidImplementation);
			}
			return eventTracker;
		}

		private static bool IsSupportedOnCurrentPlatform()
		{
			if (SharedInstance == null)
			{
				return false;
			}
			return true;
		}

		private static bool IsInitialized()
		{
			if (!IsSupportedOnCurrentPlatform())
			{
				return false;
			}
			if (!_initialized)
			{
				Debug.LogError(Constants.AdsMessageNotInitialized);
				return false;
			}
			return true;
		}

		private void Awake()
		{
			if (base.gameObject == SharedGameObject.gameObject)
			{
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		private void Update()
		{
			if (_updateOnMainThreadActions.Count <= 0)
			{
				return;
			}
			Action action;
			do
			{
				action = null;
				lock (_updateOnMainThreadActionsLock)
				{
					if (_updateOnMainThreadActions.Count > 0)
					{
						action = _updateOnMainThreadActions.Dequeue();
					}
				}
				if (action != null)
				{
					action();
				}
			}
			while (action != null);
		}

		public void EnqueueAction(Action action)
		{
			lock (_updateOnMainThreadActionsLock)
			{
				_updateOnMainThreadActions.Enqueue(action);
			}
		}

		public static void DestroyAd(string id)
		{
			if (IsInitialized())
			{
				SharedInstance.DestroyAd(id);
			}
		}

		public void _OnConfigure(string paramJson)
		{
			List<Zone> list = new List<Zone>();
			ArrayList arrayList = AdColonyJson.Decode(paramJson) as ArrayList;
			if (arrayList == null)
			{
				Debug.LogError("Unable to parse parameters in _OnConfigure, " + ((paramJson != null) ? paramJson : "null"));
				return;
			}
			IEnumerator enumerator = arrayList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					string json = (string)enumerator.Current;
					Hashtable values = AdColonyJson.Decode(json) as Hashtable;
					list.Add(new Zone(values));
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = enumerator as IDisposable) != null)
				{
					disposable.Dispose();
				}
			}
			if (Ads.OnConfigurationCompleted != null)
			{
				Ads.OnConfigurationCompleted(list);
			}
		}

		public void _OnRequestInterstitial(string paramJson)
		{
			Hashtable hashtable = AdColonyJson.Decode(paramJson) as Hashtable;
			if (hashtable == null)
			{
				Debug.LogError("Unable to parse parameters in _OnRequestInterstitial, " + ((paramJson != null) ? paramJson : "null"));
				return;
			}
			InterstitialAd adFromHashtable = GetAdFromHashtable(hashtable);
			if (adFromHashtable == null)
			{
				Debug.LogError("Unable to create ad within _OnRequestInterstitial, " + ((paramJson != null) ? paramJson : "null"));
			}
			else if (Ads.OnRequestInterstitial != null)
			{
				if (adFromHashtable != null)
				{
					Ads.OnRequestInterstitial(adFromHashtable);
				}
				else
				{
					Debug.LogError(Constants.AdsMessageErrorUnableToRebuildAd);
				}
			}
		}

		public void _OnRequestInterstitialFailed(string paramJson)
		{
			Hashtable hashtable = AdColonyJson.Decode(paramJson) as Hashtable;
			if (hashtable == null)
			{
				Debug.LogError("Unable to parse parameters in _OnRequestInterstitialFailed, " + ((paramJson != null) ? paramJson : "null"));
				return;
			}
			string obj = string.Empty;
			if (hashtable != null && hashtable.ContainsKey("zone_id"))
			{
				obj = hashtable["zone_id"] as string;
			}
			if (Ads.OnRequestInterstitialFailed != null)
			{
				Ads.OnRequestInterstitialFailed();
			}
			if (Ads.OnRequestInterstitialFailedWithZone != null)
			{
				Ads.OnRequestInterstitialFailedWithZone(obj);
			}
		}

		public void _OnOpened(string paramJson)
		{
			Hashtable hashtable = AdColonyJson.Decode(paramJson) as Hashtable;
			if (hashtable == null)
			{
				Debug.LogError("Unable to parse parameters in _OnOpened, " + ((paramJson != null) ? paramJson : "null"));
				return;
			}
			InterstitialAd adFromHashtable = GetAdFromHashtable(hashtable);
			if (adFromHashtable == null)
			{
				Debug.LogError("Unable to create ad within _OnOpened, " + ((paramJson != null) ? paramJson : "null"));
			}
			else if (Ads.OnOpened != null)
			{
				if (adFromHashtable != null)
				{
					Ads.OnOpened(adFromHashtable);
				}
				else
				{
					Debug.LogError(Constants.AdsMessageErrorUnableToRebuildAd);
				}
			}
		}

		public void _OnClosed(string paramJson)
		{
			Hashtable hashtable = AdColonyJson.Decode(paramJson) as Hashtable;
			if (hashtable == null)
			{
				Debug.LogError("Unable to parse parameters in _OnClosed, " + ((paramJson != null) ? paramJson : "null"));
				return;
			}
			InterstitialAd adFromHashtable = GetAdFromHashtable(hashtable);
			if (adFromHashtable == null)
			{
				Debug.LogError("Unable to create ad within _OnClosed, " + ((paramJson != null) ? paramJson : "null"));
				return;
			}
			if (Ads.OnClosed != null)
			{
				if (adFromHashtable != null)
				{
					Ads.OnClosed(adFromHashtable);
				}
				else
				{
					Debug.LogError(Constants.AdsMessageErrorUnableToRebuildAd);
				}
			}
			_ads.Remove(adFromHashtable.Id);
		}

		public void _OnExpiring(string paramJson)
		{
			Hashtable hashtable = AdColonyJson.Decode(paramJson) as Hashtable;
			if (hashtable == null)
			{
				Debug.LogError("Unable to parse parameters in _OnExpiring, " + ((paramJson != null) ? paramJson : "null"));
				return;
			}
			InterstitialAd adFromHashtable = GetAdFromHashtable(hashtable);
			if (adFromHashtable == null)
			{
				Debug.LogError("Unable to create ad within _OnExpiring, " + ((paramJson != null) ? paramJson : "null"));
				return;
			}
			if (Ads.OnExpiring != null)
			{
				if (adFromHashtable != null)
				{
					Ads.OnExpiring(adFromHashtable);
				}
				else
				{
					Debug.LogError(Constants.AdsMessageErrorUnableToRebuildAd);
				}
			}
			_ads.Remove(adFromHashtable.Id);
		}

		public void _OnAudioStarted(string paramJson)
		{
			Hashtable hashtable = AdColonyJson.Decode(paramJson) as Hashtable;
			if (hashtable == null)
			{
				Debug.LogError("Unable to parse parameters in _OnAudioStarted, " + ((paramJson != null) ? paramJson : "null"));
				return;
			}
			InterstitialAd adFromHashtable = GetAdFromHashtable(hashtable);
			if (adFromHashtable == null)
			{
				Debug.LogError("Unable to create ad within _OnAudioStarted, " + ((paramJson != null) ? paramJson : "null"));
			}
			else if (Ads.OnAudioStarted != null)
			{
				if (adFromHashtable != null)
				{
					Ads.OnAudioStarted(adFromHashtable);
				}
				else
				{
					Debug.LogError(Constants.AdsMessageErrorUnableToRebuildAd);
				}
			}
		}

		public void _OnAudioStopped(string paramJson)
		{
			Hashtable hashtable = AdColonyJson.Decode(paramJson) as Hashtable;
			if (hashtable == null)
			{
				Debug.LogError("Unable to parse parameters in _OnAudioStopped, " + ((paramJson != null) ? paramJson : "null"));
				return;
			}
			InterstitialAd adFromHashtable = GetAdFromHashtable(hashtable);
			if (adFromHashtable == null)
			{
				Debug.LogError("Unable to create ad within _OnAudioStopped, " + ((paramJson != null) ? paramJson : "null"));
			}
			else if (Ads.OnAudioStopped != null)
			{
				if (adFromHashtable != null)
				{
					Ads.OnAudioStopped(adFromHashtable);
				}
				else
				{
					Debug.LogError(Constants.AdsMessageErrorUnableToRebuildAd);
				}
			}
		}

		public void _OnLeftApplication(string paramJson)
		{
			Hashtable hashtable = AdColonyJson.Decode(paramJson) as Hashtable;
			if (hashtable == null)
			{
				Debug.LogError("Unable to parse parameters in _OnLeftApplication, " + ((paramJson != null) ? paramJson : "null"));
				return;
			}
			InterstitialAd adFromHashtable = GetAdFromHashtable(hashtable);
			if (adFromHashtable == null)
			{
				Debug.LogError("Unable to create ad within _OnLeftApplication, " + ((paramJson != null) ? paramJson : "null"));
			}
			else if (Ads.OnLeftApplication != null)
			{
				if (adFromHashtable != null)
				{
					Ads.OnLeftApplication(adFromHashtable);
				}
				else
				{
					Debug.LogError(Constants.AdsMessageErrorUnableToRebuildAd);
				}
			}
		}

		public void _OnClicked(string paramJson)
		{
			Hashtable hashtable = AdColonyJson.Decode(paramJson) as Hashtable;
			if (hashtable == null)
			{
				Debug.LogError("Unable to parse parameters in _OnClicked, " + ((paramJson != null) ? paramJson : "null"));
				return;
			}
			InterstitialAd adFromHashtable = GetAdFromHashtable(hashtable);
			if (adFromHashtable == null)
			{
				Debug.LogError("Unable to create ad within _OnClicked, " + ((paramJson != null) ? paramJson : "null"));
			}
			else if (Ads.OnClicked != null)
			{
				if (adFromHashtable != null)
				{
					Ads.OnClicked(adFromHashtable);
				}
				else
				{
					Debug.LogError(Constants.AdsMessageErrorUnableToRebuildAd);
				}
			}
		}

		public void _OnIAPOpportunity(string paramJson)
		{
			Hashtable hashtable = AdColonyJson.Decode(paramJson) as Hashtable;
			if (hashtable == null)
			{
				Debug.LogError("Unable to parse parameters in _OnIAPOpportunity, " + ((paramJson != null) ? paramJson : "null"));
				return;
			}
			Hashtable values = null;
			string arg = null;
			AdsIAPEngagementType arg2 = AdsIAPEngagementType.AdColonyIAPEngagementEndCard;
			if (hashtable.ContainsKey(Constants.OnIAPOpportunityAdKey))
			{
				values = AdColonyJson.Decode(hashtable[Constants.OnIAPOpportunityAdKey] as string) as Hashtable;
			}
			if (hashtable.ContainsKey(Constants.OnIAPOpportunityEngagementKey))
			{
				arg2 = (AdsIAPEngagementType)Convert.ToInt32(hashtable[Constants.OnIAPOpportunityEngagementKey]);
			}
			if (hashtable.ContainsKey(Constants.OnIAPOpportunityIapProductIdKey))
			{
				arg = hashtable[Constants.OnIAPOpportunityIapProductIdKey] as string;
			}
			InterstitialAd adFromHashtable = GetAdFromHashtable(values);
			if (adFromHashtable == null)
			{
				Debug.LogError("Unable to create ad within _OnIAPOpportunity, " + ((paramJson != null) ? paramJson : "null"));
			}
			else if (Ads.OnIAPOpportunity != null)
			{
				Ads.OnIAPOpportunity(adFromHashtable, arg, arg2);
			}
		}

		public void _OnRewardGranted(string paramJson)
		{
			string arg = null;
			bool arg2 = false;
			string arg3 = null;
			int arg4 = 0;
			Hashtable hashtable = AdColonyJson.Decode(paramJson) as Hashtable;
			if (hashtable == null)
			{
				Debug.LogError("Unable to parse parameters in _OnRewardGranted, " + ((paramJson != null) ? paramJson : "null"));
				return;
			}
			if (hashtable != null)
			{
				if (hashtable.ContainsKey(Constants.OnRewardGrantedZoneIdKey))
				{
					arg = hashtable[Constants.OnRewardGrantedZoneIdKey] as string;
				}
				if (hashtable.ContainsKey(Constants.OnRewardGrantedSuccessKey))
				{
					arg2 = Convert.ToBoolean(Convert.ToInt32(hashtable[Constants.OnRewardGrantedSuccessKey]));
				}
				if (hashtable.ContainsKey(Constants.OnRewardGrantedNameKey))
				{
					arg3 = hashtable[Constants.OnRewardGrantedNameKey] as string;
				}
				if (hashtable.ContainsKey(Constants.OnRewardGrantedAmountKey))
				{
					arg4 = Convert.ToInt32(hashtable[Constants.OnRewardGrantedAmountKey]);
				}
			}
			if (Ads.OnRewardGranted != null)
			{
				Ads.OnRewardGranted(arg, arg2, arg3, arg4);
			}
		}

		public void _OnCustomMessageReceived(string paramJson)
		{
			string arg = null;
			string arg2 = null;
			Hashtable hashtable = AdColonyJson.Decode(paramJson) as Hashtable;
			if (hashtable == null)
			{
				Debug.LogError("Unable to parse parameters in _OnCustomMessageReceived, " + ((paramJson != null) ? paramJson : "null"));
				return;
			}
			if (hashtable != null)
			{
				if (hashtable.ContainsKey(Constants.OnCustomMessageReceivedTypeKey))
				{
					arg = hashtable[Constants.OnCustomMessageReceivedTypeKey] as string;
				}
				if (hashtable.ContainsKey(Constants.OnCustomMessageReceivedMessageKey))
				{
					arg2 = hashtable[Constants.OnCustomMessageReceivedMessageKey] as string;
				}
			}
			if (Ads.OnCustomMessageReceived != null)
			{
				Ads.OnCustomMessageReceived(arg, arg2);
			}
		}

		private InterstitialAd GetAdFromHashtable(Hashtable values)
		{
			string text = null;
			if (values != null && values.ContainsKey("id"))
			{
				text = values["id"] as string;
			}
			InterstitialAd interstitialAd = null;
			if (text != null)
			{
				if (_ads.ContainsKey(text))
				{
					interstitialAd = _ads[text];
					interstitialAd.UpdateValues(values);
				}
				else
				{
					interstitialAd = new InterstitialAd(values);
					_ads[text] = interstitialAd;
				}
			}
			return interstitialAd;
		}
	}
}
