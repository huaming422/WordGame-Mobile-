using System;
using UnityEngine;

public class TapResearch : MonoBehaviour
{
	public delegate void PlacementDelegate(TRPlacement placement);

	public delegate void TRRewardDelegate(TRReward reward);

	public delegate void TRSurveyModalDelegate(TRPlacement placement);

	private static AndroidJavaClass _unityBridge;

	private const string version = "2.0.3";

	private static TapResearch _instance;

	public static PlacementDelegate OnPlacementReady;

	public static TRRewardDelegate OnReceiveReward;

	public static TRSurveyModalDelegate OnSurveyWallOpened;

	public static TRSurveyModalDelegate OnSurveyWallDismissed;

	private static bool _pluginInitialized;

	private static AndroidJavaClass _unityPlayer;

	private static void InitializeInstance()
	{
		if (_instance == null)
		{
			_instance = UnityEngine.Object.FindObjectOfType(typeof(TapResearch)) as TapResearch;
			if (_instance == null)
			{
				_instance = new GameObject("TapResearch").AddComponent<TapResearch>();
			}
		}
	}

	private void Awake()
	{
		base.name = "TapResearch";
		UnityEngine.Object.DontDestroyOnLoad(base.transform.gameObject);
	}

	private void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.transform.gameObject);
	}

	public void OnTapResearchPlacementReady(string args)
	{
		Debug.Log("OnTapResearchPlacementReady");
		if (OnPlacementReady != null)
		{
			Debug.Log("On placement ready called with args - " + args);
			TRPlacement tRPlacement = JsonUtility.FromJson<TRPlacement>(args);
			Debug.Log(tRPlacement.PlacementIdentifier);
			OnPlacementReady(tRPlacement);
		}
	}

	public void TapResearchOnSurveyWallOpened(string args)
	{
		if (OnSurveyWallOpened != null)
		{
			TRPlacement placement = JsonUtility.FromJson<TRPlacement>(args);
			OnSurveyWallOpened(placement);
		}
	}

	public void TapResearchOnSurveyWallDismissed(string args)
	{
		if (OnSurveyWallDismissed != null)
		{
			TRPlacement placement = JsonUtility.FromJson<TRPlacement>(args);
			OnSurveyWallDismissed(placement);
		}
	}

	public void OnTapResearchDidReceiveReward(string args)
	{
		if (OnReceiveReward != null)
		{
			TRReward reward = JsonUtility.FromJson<TRReward>(args);
			OnReceiveReward(reward);
		}
	}

	public static void Configure(string apiToken)
	{
		InitializeInstance();
		AndroidConfigure(apiToken, "2.0.3");
	}

	private static void InitializeAndroidPlugin()
	{
		_unityBridge = new AndroidJavaClass("com.tapr.unitybridge.TRUnityBridge");
		if (_unityBridge == null)
		{
			Debug.LogError("********************* Can't create AndroidJavaClass ***************************");
			return;
		}
		IntPtr intPtr = AndroidJNI.FindClass("com/tapr/sdk/TapResearch");
		if (intPtr != IntPtr.Zero)
		{
			AndroidJNI.DeleteLocalRef(intPtr);
			_unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			_pluginInitialized = true;
		}
		else
		{
			Debug.LogError("TapResearch android config error. Make sure you've included both tapresearch.jar and unitybridge.jar in your Unity project's Assets/Plugins/Android folder.");
		}
	}

	private static void AndroidConfigure(string apiToken, string version)
	{
		if (!_pluginInitialized)
		{
			InitializeAndroidPlugin();
			AndroidJavaObject @static = _unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			_unityBridge.CallStatic("configure", apiToken, @static, version);
		}
	}

	private static bool isInitialized()
	{
		if (!_pluginInitialized)
		{
			Debug.Log("Please call `Configure (string apiToken)` before making any ");
		}
		return _pluginInitialized;
	}

	public static void InitPlacement(string placementIdentifier)
	{
		if (isInitialized())
		{
			_unityBridge.CallStatic("initPlacement", placementIdentifier);
		}
	}

	public static void ShowSurveyWall(string placementIdentifier)
	{
		if (isInitialized())
		{
			_unityBridge.CallStatic("showSurveyWall", placementIdentifier);
		}
	}

	public static void SetUniqueUserIdentifier(string userIdentifier)
	{
		if (isInitialized())
		{
			_unityBridge.CallStatic("setUniqueUserIdentifier", userIdentifier);
		}
	}

	public static void SetDebugMode(bool debugMode)
	{
		if (isInitialized())
		{
			_unityBridge.CallStatic("setDebugMode", debugMode);
		}
	}

	public static void SetNavigationBarColor(string hexColor)
	{
		if (isInitialized())
		{
			_unityBridge.CallStatic("setNavigationBarColor", hexColor);
		}
	}

	public static void SetNavigationBarText(string text)
	{
		if (isInitialized())
		{
			_unityBridge.CallStatic("setNavigationBarText", text);
		}
	}

	public static void SetNavigationBarTextColor(string hexColor)
	{
		if (isInitialized())
		{
			_unityBridge.CallStatic("setNavigationBarTextColor", hexColor);
		}
	}
}
