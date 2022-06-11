using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AmzInAppPurchManager : IPurchManager
{
	private string sharekey = "2:qbko0uBUerE-1HfmIbuRZ90gVqjGM-9PGZU6SLy9dCtqA518q3CxcxE67AdpWn2O:BFBS2srBFg1nPbAAbuWdAQ==";

	private string testRvsUrl = string.Empty;

	private string rvsUrl = "https://appstore-sdk.amazon.com/version/1.0/verifyReceiptId/developer/";

	public static string[] myGoods = new string[5] { "com.sample.wordgames.300coins", "com.sample.wordgames.1500coins", "com.sample.wordgames.3600coins", "com.sample.wordgames.10000coins", "com.sample.wordgames.removeads" };

	private AndroidJavaClass _amzIapclass;

	private Dictionary<string, UnityAction<bool, string>> m_AllCallback = new Dictionary<string, UnityAction<bool, string>>();

	private Dictionary<string, string> _requestIdToSuk = new Dictionary<string, string>();

	public void Init()
	{
		try
		{
			SingleObject<SDKManager>.instance.AddEvent("amziap", OnPurchaseCallBack);
			_amzIapclass = new AndroidJavaClass("assnoc.amziapplugin.AmzIAPMannager");
			_amzIapclass.CallStatic<bool>("Init", new object[0]);
			_amzIapclass.CallStatic("UpdatePurchas");
			Debug.LogWarning("assnoc.amziapplugin.AmzIAPMannager is init finish");
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Init assnoc.amziapplugin.AmzIAPMannager is error:" + ex.Message);
		}
	}

	public void Purch(string id, UnityAction<bool, string> callback)
	{
		if (Application.platform == RuntimePlatform.WindowsEditor)
		{
			if (callback != null)
			{
				callback(true, id);
			}
			return;
		}
		if (_amzIapclass == null)
		{
			if (callback != null)
			{
				callback(false, "not init unity iap");
			}
			return;
		}
		string text = GameGoodIdToAmzId(int.Parse(id));
		if (!m_AllCallback.ContainsKey(text))
		{
			m_AllCallback.Add(text, callback);
			string key = _amzIapclass.CallStatic<string>("Purchas", new object[1] { text });
			_requestIdToSuk.Add(key, text);
		}
		else if (callback != null)
		{
			callback(false, "Double buy");
		}
	}

	private void OnPurchaseCallBack(int code, string json)
	{
		Hashtable hashtable = json.DecodeJson();
		if (hashtable == null)
		{
			return;
		}
		string @string = hashtable.GetString("requestId");
		string string2 = hashtable.GetString("userId");
		string string3 = hashtable.GetString("receiptId");
		string text = hashtable.GetString("suk");
		if (_requestIdToSuk.ContainsKey(@string))
		{
			text = _requestIdToSuk[@string];
			_requestIdToSuk.Remove(@string);
		}
		UnityAction<bool, string> unityAction = null;
		if (m_AllCallback.ContainsKey(text))
		{
			unityAction = m_AllCallback[text];
			m_AllCallback.Remove(text);
		}
		if (code == 1 || code == 2)
		{
			int num = AzmIdToGameGoodId(text);
			if (unityAction != null)
			{
				unityAction(true, num.ToString());
			}
		}
		else if (unityAction != null)
		{
			unityAction(false, "buy error:" + text);
		}
	}

	private IEnumerator SendVerify(string userId, string receiptId, string requestId)
	{
		string host = ((!AppConst.IsIAPTestMode) ? rvsUrl : testRvsUrl);
		string url = string.Format("{0}{1}/user/{2}/receiptId/{3}", host, sharekey, userId, receiptId);
		WWW www = new WWW(url);
		yield return www;
		if (string.IsNullOrEmpty(www.error))
		{
			AmzIapRecoders.RemoveLineByReceiptId(receiptId);
			Hashtable data = www.text.DecodeJson();
			string @string = data.GetString("productId");
			if (m_AllCallback.ContainsKey(@string))
			{
				UnityAction<bool, string> unityAction = m_AllCallback[@string];
				m_AllCallback.Remove(@string);
				if (unityAction != null)
				{
					unityAction(true, @string);
				}
			}
		}
		else if (string.IsNullOrEmpty(requestId))
		{
		}
	}

	private string GameGoodIdToAmzId(int gameGoodId)
	{
		if (gameGoodId > myGoods.Length)
		{
			return string.Empty;
		}
		return myGoods[gameGoodId - 1];
	}

	private int AzmIdToGameGoodId(string amzId)
	{
		for (int i = 0; i < myGoods.Length; i++)
		{
			if (amzId == myGoods[i])
			{
				return i + 1;
			}
		}
		return -1;
	}

	public void CheckGoodsIsPurch(string id, UnityAction<bool> callback)
	{
		string goodsId = GameGoodIdToAmzId(int.Parse(id));
		SingleObject<WWWResouceMannager>.instance.CheckGoodsIsPurch(SingleObject<SDKManager>.instance.GetMacAddr(), goodsId, Application.identifier, callback);
	}

	public void AddGoodPurch(string id, UnityAction<bool> callback)
	{
		string goodsId = GameGoodIdToAmzId(int.Parse(id));
		SingleObject<WWWResouceMannager>.instance.AddGoodsPurch(SingleObject<SDKManager>.instance.GetMacAddr(), goodsId, Application.identifier, callback);
	}
}
