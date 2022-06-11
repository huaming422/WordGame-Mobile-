using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GoogleInAppPurchManager : IPurchManager
{
	public static string[] myGoods = new string[5] { "com.sample.wordgames.300coins", "com.sample.wordgames.1500coins", "com.sample.wordgames.3600coins", "com.sample.wordgames.10000coins", "com.sample.wordgames.removeads" };

	private Dictionary<string, UnityAction<bool, string>> m_AllCallback = new Dictionary<string, UnityAction<bool, string>>();

	public void Init()
	{
		SingleObject<SDKManager>.instance.AddEvent("googleIap", DealPurchaseReuslt);
	}

	public void Purch(string id, UnityAction<bool, string> callback)
	{
		string text = GameGoodsIdToGoogleId(int.Parse(id));
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		if (Application.platform == RuntimePlatform.WindowsEditor)
		{
			if (callback != null)
			{
				callback(true, id);
			}
		}
		else if (!m_AllCallback.ContainsKey(text))
		{
			m_AllCallback.Add(text, callback);
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			if (@static == null)
			{
				callback(false, "not init unity iap");
				return;
			}
			@static.Call("Pay", text);
		}
		else if (callback != null)
		{
			callback(false, "Double buy");
		}
	}

	private string GameGoodsIdToGoogleId(int gameGoodId)
	{
		if (gameGoodId > myGoods.Length)
		{
			return string.Empty;
		}
		return myGoods[gameGoodId - 1];
	}

	private void OnPurchaseCallBack(int result, string sku)
	{
	}

	public void DealPurchaseReuslt(int result_Id, string id)
	{
		bool arg = result_Id == 1;
		UnityAction<bool, string> value = null;
		if (m_AllCallback.TryGetValue(id, out value))
		{
			m_AllCallback.Remove(id);
			if (value != null)
			{
				value(arg, id.ToString());
			}
		}
	}

	public void CheckGoodsIsPurch(string id, UnityAction<bool> callback)
	{
	}

	public void AddGoodPurch(string id, UnityAction<bool> callback)
	{
	}
}
