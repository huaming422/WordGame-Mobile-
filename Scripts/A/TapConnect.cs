using TapjoyUnity;
using UnityEngine;

public class TapConnect : MonoBehaviour
{
	private string sdkKey = "Jk3xOflNTriCv_GPsTSGugECcsWFOx4Pi4k5SgHoitrnxKAnTxv31fs3f08A";

	private void Awake()
	{
		if (!Tapjoy.IsConnected && IsActiveSdkGo())
		{
			Tapjoy.Connect(sdkKey);
			Tapjoy.SetDebugEnabled(false);
			Tapjoy.OnConnectSuccess += Tapjoy_OnConnectSuccess;
			Tapjoy.OnConnectFailure += Tapjoy_OnConnectFailure;
		}
	}

	private void Tapjoy_OnConnectFailure()
	{
		Debug.Log("Tap Tapjoy_OnConnectFailure ");
		Tapjoy.Connect();
	}

	private void Tapjoy_OnConnectSuccess()
	{
		Debug.Log("Tap Tapjoy_OnConnectSuccess ");
	}

	private bool IsActiveSdkGo()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		if (!@static.Call<bool>("Door", new object[1] { "false==cut" }))
		{
			return true;
		}
		string key = "IsActiveTapjoyAdSdk";
		if (PlayerPrefs.HasKey(key))
		{
			return PlayerPrefs.GetInt(key) == 1;
		}
		int num = Mathf.CeilToInt(Random.value * 1000f);
		if (num < 720)
		{
			PlayerPrefs.SetInt(key, 0);
			return false;
		}
		PlayerPrefs.SetInt(key, 1);
		return true;
	}
}
