using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Events;

public class SDKManager : SingleObject<SDKManager>
{
	private AndroidJavaClass m_EmailFactroy;

	private UnityAction _onTakePhoto;

	private UnityAction<string> notifiRegistCallback;

	private Dictionary<string, UnityAction<int, string>> _sdkEvents = new Dictionary<string, UnityAction<int, string>>();

	private void Awake()
	{
		try
		{
			m_EmailFactroy = new AndroidJavaClass("com.Aiunity3dTwo.player.EmailFactroy");
			Debug.LogWarning("Android java class is init finish");
			InitSdkManager();
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Init jave class is error:" + ex.Message);
		}
	}

	public void Init()
	{
	}

	private void Start()
	{
		AddEvent("notifi", DoReciveNotifiMessage);
		AddEvent("takephoto", DoReciveTakePhotoMessage);
	}

	private void InitSdkManager()
	{
		if (m_EmailFactroy != null)
		{
			m_EmailFactroy.CallStatic("RegistRecive");
			m_EmailFactroy.CallStatic("SetStrictMode");
		}
	}

	public void SendEmail(string emailto, string title, string content)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Uri uri = new Uri(string.Format("mailto:{0}?subject={1}&body={2}", emailto, title, content));
			Application.OpenURL(uri.AbsoluteUri);
		}
		if (Application.platform == RuntimePlatform.Android)
		{
			if (m_EmailFactroy != null)
			{
				Debug.Log("call send Email for android");
				m_EmailFactroy.CallStatic("SendEmail", emailto, title, content);
			}
			else
			{
				Debug.LogWarning("android email class dont init");
			}
		}
	}

	public void QuitGame()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			m_EmailFactroy.CallStatic("QuitGame");
		}
		else
		{
			Application.Quit();
		}
	}

	public void Vibrate(float time)
	{
		if (m_EmailFactroy != null)
		{
			m_EmailFactroy.CallStatic("Vibrator", time);
		}
	}

	public bool IsInstallApp(string packageName)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			if (m_EmailFactroy != null)
			{
				return m_EmailFactroy.CallStatic<bool>("IsInstallApp", new object[1] { packageName });
			}
			return false;
		}
		return false;
	}

	public void TakePhoto(int type, string fileName, UnityAction callback)
	{
		if (m_EmailFactroy != null)
		{
			_onTakePhoto = callback;
			if (Application.platform == RuntimePlatform.Android && m_EmailFactroy != null)
			{
				m_EmailFactroy.CallStatic("TakePhoto", type, fileName);
			}
		}
	}

	private void DoReciveTakePhotoMessage(int code, string data)
	{
		if (code == 1)
		{
			_onTakePhoto.MyInvoke();
		}
	}

	public void CopyToClipBoard(string data)
	{
		if (m_EmailFactroy != null)
		{
			m_EmailFactroy.CallStatic("CopyToClipBoard", data);
		}
	}

	public void GetGoogleAdId()
	{
		if (Application.isMobilePlatform && Application.platform == RuntimePlatform.Android && m_EmailFactroy != null)
		{
			m_EmailFactroy.CallStatic("GetGoogleAdId");
		}
	}

	public string GetMacAddr()
	{
		if (Application.isMobilePlatform)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				if (m_EmailFactroy == null)
				{
					return string.Empty;
				}
				return m_EmailFactroy.CallStatic<string>("GetMacaddr", new object[0]);
			}
			return string.Empty;
		}
		NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
		if (allNetworkInterfaces == null || allNetworkInterfaces.Length == 0)
		{
			return string.Empty;
		}
		return allNetworkInterfaces[0].GetPhysicalAddress().ToString();
	}

	public void Notification(int time, string title, string content, UnityAction<string> onRegistCallback)
	{
		if (m_EmailFactroy != null)
		{
			notifiRegistCallback = onRegistCallback;
			m_EmailFactroy.CallStatic("Notifi", time * 1000, title, content);
		}
	}

	private void DoReciveNotifiMessage(int code, string data)
	{
		if (code == 1 && notifiRegistCallback != null)
		{
			notifiRegistCallback(data);
		}
	}

	public bool SavePicToDICM(string picPath)
	{
		if (m_EmailFactroy == null)
		{
			return true;
		}
		try
		{
			return m_EmailFactroy.CallStatic<bool>("SavePicToPhoto", new object[2]
			{
				picPath,
				Application.productName
			});
		}
		catch (Exception ex)
		{
			Util.LogWarning("SavePicToDICM Error:" + ex.ToString());
			return false;
		}
	}

	public void ShareText(string title, string text)
	{
		if (m_EmailFactroy != null)
		{
			m_EmailFactroy.CallStatic("ShareText", title, text);
		}
	}

	public void SharePic(string picPath, string title, string text)
	{
		if (m_EmailFactroy != null)
		{
			m_EmailFactroy.CallStatic("SharePic", picPath, title, text);
		}
	}

	public void AddEvent(string tag, UnityAction<int, string> callback)
	{
		if (callback != null)
		{
			if (_sdkEvents.ContainsKey(tag))
			{
				_sdkEvents.Remove(tag);
			}
			_sdkEvents.Add(tag, callback);
		}
	}

	public void RemoveEvent(string tag)
	{
		if (_sdkEvents.ContainsKey(tag))
		{
			_sdkEvents.Remove(tag);
		}
	}

	public void AndroidToUnity(string json)
	{
		Util.LogWarning("AndroidToUnity message ---->>>:" + json);
		if (string.IsNullOrEmpty(json))
		{
			return;
		}
		Hashtable data = json.DecodeJson();
		string @string = data.GetString("tag");
		if (!string.IsNullOrEmpty(@string))
		{
			if (_sdkEvents.ContainsKey(@string))
			{
				UnityAction<int, string> unityAction = _sdkEvents[@string];
				if (unityAction != null)
				{
					unityAction(data.GetInt("code"), data.GetString("data"));
				}
			}
			return;
		}
		string string2 = data.GetString("type");
		if (string2 == null)
		{
			Debug.Log("type  == null  return");
		}
		else if (_sdkEvents.ContainsKey(string2))
		{
			Debug.Log("AndroidToUnityJson receive data from " + string2);
			Hashtable data2 = data.GetString("data").DecodeJson();
			int @int = data2.GetInt("result");
			string string3 = data2.GetString("sku");
			UnityAction<int, string> unityAction2 = _sdkEvents[string2];
			if (unityAction2 != null)
			{
				unityAction2(@int, string3);
			}
		}
	}
}
