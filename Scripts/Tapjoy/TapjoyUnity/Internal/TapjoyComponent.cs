using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace TapjoyUnity.Internal
{
	public class TapjoyComponent : MonoBehaviour
	{
		private enum InternalEventType
		{
			RemovePlacement,
			RemoveActionRequest
		}

		private struct InternalEvent
		{
			public InternalEventType type;

			public string data;

			public InternalEvent(InternalEventType _type, string _data)
			{
				type = _type;
				data = _data;
			}
		}

		private const string GAME_OBJECT_NAME = "TapjoyUnity";

		private const string DISABLE_ADVERTISING_ID_CHECK = "TJC_OPTION_DISABLE_ADVERTISING_ID_CHECK";

		private const string DISABLE_PERSISTENT_IDS = "TJC_OPTION_DISABLE_PERSISTENT_IDS";

		private const string FYBER_APP_ID = "TJC_OPTION_FYBER_APP_ID";

		private const string FYBER_APP_TOKEN = "TJC_OPTION_FYBER_APP_TOKEN";

		private const string FYBER_USER_ID = "TJC_OPTION_FYBER_USER_ID";

		private static bool applicationPaused = true;

		private static bool isConnecting = false;

		private static bool triedConnecting = false;

		private static PlatformSettings app;

		private static GameObject singletonGameObject;

		private Dictionary<string, object> lastConnectFlags;

		private static Queue events = Queue.Synchronized(new Queue());

		[HideInInspector]
		public TapjoySettings settings;

		public static TapjoyComponent FindInstance()
		{
			return UnityEngine.Object.FindObjectOfType(typeof(TapjoyComponent)) as TapjoyComponent;
		}

		private void Awake()
		{
			switch (Application.platform)
			{
			case RuntimePlatform.Android:
				app = settings.AndroidSettings;
				break;
			case RuntimePlatform.IPhonePlayer:
				app = settings.IosSettings;
				break;
			default:
				Debug.LogWarning("Tapjoy doesn't support " + Application.platform);
				app = new PlatformSettings();
				break;
			}
			ApiBinding.OnInstanceSet = OnApiBindingSet;
			EnsureSingleton();
		}

		private void OnApiBindingSet()
		{
			ApiBinding.Instance.SetDebugEnabled(settings.DebugEnabled);
			if (Application.platform == RuntimePlatform.Android)
			{
				ApiBinding.Instance.SetGcmSender(app.PushKey);
			}
		}

		private void EnsureSingleton()
		{
			if (!(base.gameObject == null))
			{
				if (singletonGameObject == null)
				{
					singletonGameObject = base.gameObject;
					UnityEngine.Object.DontDestroyOnLoad(singletonGameObject);
				}
				else if (singletonGameObject != base.gameObject)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}

		internal void Reconnect()
		{
			if (!isConnecting && !Tapjoy.IsConnected)
			{
				if (!triedConnecting)
				{
					Debug.LogWarning("Must first call connect with an SDK key.");
				}
				else
				{
					ApiBinding.Instance.Connect(app.SdkKey, lastConnectFlags);
				}
			}
		}

		internal bool ConnectManually()
		{
			return ConnectManually(app.SdkKey, null);
		}

		internal bool ConnectManually(string sdkKey)
		{
			return ConnectManually(sdkKey, null);
		}

		internal bool ConnectManually(string sdkKey, Dictionary<string, object> flags)
		{
			if (isConnecting)
			{
				return false;
			}
			if (Tapjoy.IsConnected)
			{
				return false;
			}
			app.SdkKey = sdkKey;
			return ConnectInternal(flags);
		}

		private bool ConnectInternal(Dictionary<string, object> connectFlags)
		{
			if (!app.Valid)
			{
				if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
				{
					Debug.LogWarning("Please check if you applied correct SDK key.");
				}
				return false;
			}
			Tapjoy.OnConnectSuccessInternal -= HandleOnConnectSuccess;
			Tapjoy.OnConnectSuccessInternal += HandleOnConnectSuccess;
			Tapjoy.OnConnectFailureInternal -= HandleOnConnectFailure;
			Tapjoy.OnConnectFailureInternal += HandleOnConnectFailure;
			if (connectFlags == null)
			{
				connectFlags = new Dictionary<string, object>();
			}
			if (!connectFlags.ContainsKey("TJC_OPTION_DISABLE_ADVERTISING_ID_CHECK") && app.DisableAdvertisingId)
			{
				connectFlags.Add("TJC_OPTION_DISABLE_ADVERTISING_ID_CHECK", app.DisableAdvertisingId);
			}
			if (!connectFlags.ContainsKey("TJC_OPTION_DISABLE_PERSISTENT_IDS") && app.DisablePersistentIds)
			{
				connectFlags.Add("TJC_OPTION_DISABLE_PERSISTENT_IDS", app.DisablePersistentIds);
			}
			lastConnectFlags = connectFlags;
			isConnecting = true;
			triedConnecting = true;
			ApiBinding.Instance.Connect(app.SdkKey, connectFlags);
			ApiBinding.Instance.StartSession();
			SceneUsageTracking.Enabled = true;
			return true;
		}

		private void OnDestroy()
		{
			Tapjoy.OnConnectSuccessInternal -= HandleOnConnectSuccess;
			Tapjoy.OnConnectFailureInternal -= HandleOnConnectFailure;
			isConnecting = false;
		}

		private void HandleOnConnectSuccess()
		{
			isConnecting = false;
			Invoke("InitMediation", 0f);
		}

		private void HandleOnConnectFailure()
		{
			isConnecting = false;
		}

		private void Start()
		{
			if (settings.AutoConnectEnabled)
			{
				ConnectManually();
			}
			if (applicationPaused)
			{
				applicationPaused = false;
			}
		}

		private void Update()
		{
			if (events.Count <= 0)
			{
				return;
			}
			try
			{
				InternalEvent internalEvent = (InternalEvent)events.Dequeue();
				if (internalEvent.type == InternalEventType.RemovePlacement)
				{
					ApiBinding.Instance.RemovePlacement(internalEvent.data);
				}
				else if (internalEvent.type == InternalEventType.RemoveActionRequest)
				{
					ApiBinding.Instance.RemoveActionRequest(internalEvent.data);
				}
			}
			catch (InvalidOperationException)
			{
			}
		}

		private void OnApplicationPause(bool paused)
		{
			if (app == null || applicationPaused == paused)
			{
				return;
			}
			applicationPaused = paused;
			if (app.Valid && triedConnecting)
			{
				if (paused)
				{
					ApiBinding.Instance.EndSession();
				}
				else
				{
					ApiBinding.Instance.StartSession();
				}
			}
			ForegroundRealtimeClock.OnApplicationPause(paused);
		}

		private void OnApplicationQuit()
		{
			if (!applicationPaused)
			{
				applicationPaused = true;
				if (app.Valid)
				{
					ApiBinding.Instance.EndSession();
					SceneUsageTracking.Enabled = false;
				}
			}
		}

		private void OnNativeConnectCallback(string commaDelimitedMessage)
		{
			Tapjoy.DispatchConnectEvent(commaDelimitedMessage);
		}

		private void OnNativeSetUserIDCallback(string commaDelimitedMessage)
		{
			Tapjoy.DispatchSetUserIDEvent(commaDelimitedMessage);
		}

		private void OnNativeCurrencyCallback(string commaDelimitedMessage)
		{
			Tapjoy.DispatchCurrencyEvent(commaDelimitedMessage);
		}

		private void OnNativePlacementCallback(string commaDelimitedMessage)
		{
			TJPlacement.DispatchPlacementEvent(commaDelimitedMessage);
		}

		private void OnNativeVideoCallback(string commaDelimitedMessage)
		{
			Tapjoy.DispatchVideoEvent(commaDelimitedMessage);
		}

		private void OnNativePlacementVideoCallback(string commaDelimitedMessage)
		{
			TJPlacement.DispatchPlacementVideoEvent(commaDelimitedMessage);
		}

		public static void RemovePlacement(string placementID)
		{
			events.Enqueue(new InternalEvent(InternalEventType.RemovePlacement, placementID));
		}

		public static void RemoveActionRequest(string requestID)
		{
			events.Enqueue(new InternalEvent(InternalEventType.RemoveActionRequest, requestID));
		}

		public static TapjoyRuntimeCallbacks GetTapjoyRuntimeCallbacks()
		{
			GameObject gameObject = GameObject.Find("TapjoyUnity");
			if (gameObject != null)
			{
				return gameObject.GetComponent<TapjoyRuntimeCallbacks>();
			}
			return null;
		}

		private void InitMediation()
		{
			Assembly assembly = null;
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			Assembly[] array = assemblies;
			foreach (Assembly assembly2 in array)
			{
				if (assembly2.GetName().Name == "Assembly-CSharp")
				{
					assembly = assembly2;
					break;
				}
			}
			if (assembly == null)
			{
				return;
			}
			FyberSettings fyberSettings = null;
			switch (Application.platform)
			{
			case RuntimePlatform.Android:
				fyberSettings = settings.AndroidSettings.FyberMediationSettings;
				break;
			case RuntimePlatform.IPhonePlayer:
				fyberSettings = settings.IosSettings.FyberMediationSettings;
				break;
			}
			if (fyberSettings == null || !fyberSettings.Valid)
			{
				return;
			}
			if (GetTapjoyRuntimeCallbacks() != null && GetTapjoyRuntimeCallbacks().GetFyberUserId() != "")
			{
				fyberSettings.UserId = GetTapjoyRuntimeCallbacks().GetFyberUserId();
			}
			Type type = assembly.GetType("TapjoyUnity.Mediation.TapjoyFyberInitializer");
			if (type == null)
			{
				Debug.Log("Tapjoy could not find TapjoyUnity.Mediation.TapjoyFyberInitializer in unity assembly.");
				return;
			}
			MethodInfo method = type.GetMethod("Init");
			if (method != null)
			{
				method.Invoke(null, new object[4] { fyberSettings.AppId, fyberSettings.AppToken, fyberSettings.UserId, settings.DebugEnabled });
			}
			else
			{
				Debug.Log("Could not find TapjoyUnity.Mediation.TapjoyFyberInitializer.Init() in unity assembly.");
			}
		}
	}
}
