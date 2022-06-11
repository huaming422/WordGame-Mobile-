using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WWWResouceMannager : SingleObject<WWWResouceMannager>
{
	private class RequestItem
	{
		public string url;

		public List<UnityAction<object>> Callbacks;

		public Type type;

		public float timeOut = -1f;
	}

	private bool IsLoading;

	private Dictionary<string, RequestItem> allRequest = new Dictionary<string, RequestItem>();

	private Dictionary<string, object> allLoadedRequest = new Dictionary<string, object>();

	private Dictionary<string, Sprite> m_AllLoaedSprite = new Dictionary<string, Sprite>();

	private Dictionary<string, UnityAction<Sprite>> m_LoadingList = new Dictionary<string, UnityAction<Sprite>>();

	private Dictionary<string, List<UnityAction<int>>> uploadings = new Dictionary<string, List<UnityAction<int>>>();

	private Dictionary<string, List<UnityAction<int, object>>> downloadings = new Dictionary<string, List<UnityAction<int, object>>>();

	public void LoadRes(string url, Type type, UnityAction<object> callback, float timeOut = -1f)
	{
		if (allLoadedRequest.ContainsKey(url))
		{
			callback.MyInvoke(allLoadedRequest[url]);
			return;
		}
		if (allRequest.ContainsKey(url))
		{
			allRequest[url].Callbacks.Add(callback);
			return;
		}
		RequestItem requestItem = new RequestItem();
		requestItem.timeOut = timeOut;
		requestItem.url = url;
		requestItem.type = type;
		requestItem.Callbacks = new List<UnityAction<object>>();
		requestItem.Callbacks.Add(callback);
		allRequest.Add(url, requestItem);
		StartCoroutine(StartLoad(requestItem));
	}

	private IEnumerator StartLoad(RequestItem tempRequest)
	{
		WWW www = new WWW(tempRequest.url);
		float startTime = Time.realtimeSinceStartup;
		bool isLoadSucess3 = false;
		while (true)
		{
			if (www.isDone)
			{
				isLoadSucess3 = true;
				break;
			}
			if (Time.realtimeSinceStartup - startTime > tempRequest.timeOut && tempRequest.timeOut > 0f)
			{
				isLoadSucess3 = false;
				break;
			}
			yield return null;
		}
		allRequest.Remove(tempRequest.url);
		isLoadSucess3 = string.IsNullOrEmpty(www.error) && isLoadSucess3;
		object temp = null;
		if (isLoadSucess3)
		{
			if (typeof(Texture2D) == tempRequest.type)
			{
				temp = www.texture;
			}
			if (tempRequest.type == typeof(string))
			{
				temp = www.text;
			}
			if (tempRequest.type == typeof(byte[]))
			{
				temp = www.bytes;
			}
			if (tempRequest.type == typeof(AudioClip))
			{
				temp = www.GetAudioClip();
			}
			allLoadedRequest.Add(tempRequest.url, temp);
		}
		else
		{
			Util.LogWarning(string.Format("wwwResManger LoadRes from {0}Error------>{1}", tempRequest.url, www.error));
		}
		List<UnityAction<object>> callbacks = tempRequest.Callbacks;
		for (int i = 0; i < callbacks.Count; i++)
		{
			UnityAction<object> action = callbacks[i];
			try
			{
				action.MyInvoke(temp);
			}
			catch (Exception ex)
			{
				Util.LogWarning(string.Format("wwwResManger LoadRes from {0} do callback Error------>{1}", tempRequest.url, ex.ToString()));
			}
		}
	}

	public void RemoveCache(string url)
	{
		if (m_AllLoaedSprite.ContainsKey(url))
		{
			m_AllLoaedSprite.Remove(url);
		}
	}

	public void SetIcon(string url, Image image)
	{
		LoadSprite(url, delegate(Sprite sprite)
		{
			image.sprite = sprite;
		});
	}

	public void LoadSprite(string url, UnityAction<Sprite> callBack)
	{
		Sprite value = null;
		if (m_AllLoaedSprite.TryGetValue(url, out value) && callBack != null)
		{
			callBack(value);
			return;
		}
		UnityAction<Sprite> value2 = null;
		if (m_LoadingList.TryGetValue(url, out value2))
		{
			if (callBack != null)
			{
				Dictionary<string, UnityAction<Sprite>> loadingList;
				string key;
				(loadingList = m_LoadingList)[key = url] = (UnityAction<Sprite>)Delegate.Combine(loadingList[key], callBack);
			}
		}
		else
		{
			m_LoadingList.Add(url, callBack);
			StartCoroutine(LoadSprite(url));
		}
	}

	private IEnumerator LoadSprite(string url)
	{
		Util.Log("request : " + url);
		WWW www = new WWW(url);
		yield return www;
		if (string.IsNullOrEmpty(www.error))
		{
			LoadImageCalBack(url, www.texture);
			yield break;
		}
		Util.Log("request error : " + www.error);
		LoadImageCalBack(url, null);
	}

	private void LoadImageCalBack(string url, UnityEngine.Object obj)
	{
		UnityAction<Sprite> value = null;
		if (obj == null && m_LoadingList.ContainsKey(url))
		{
			m_LoadingList[url](null);
			m_LoadingList.Remove(url);
			return;
		}
		Sprite sprite = obj as Sprite;
		if (sprite == null)
		{
			Texture2D texture2D = obj as Texture2D;
			if (null != texture2D)
			{
				sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 100f);
			}
		}
		if (sprite != null)
		{
			m_AllLoaedSprite.Add(url, sprite);
			if (m_LoadingList.TryGetValue(url, out value))
			{
				value(sprite);
			}
		}
		m_LoadingList.Remove(url);
	}

	public void UpLoadFile(string cmd, string gameId, byte[] data, Dictionary<string, string> otherData = null, UnityAction<int> callback = null)
	{
		if (data.IsEmpty())
		{
			callback.MyInvoke(0);
			return;
		}
		List<UnityAction<int>> list = null;
		string key = cmd + gameId;
		if (uploadings.ContainsKey(key))
		{
			list = uploadings[key];
			list.Add(callback);
			return;
		}
		list = new List<UnityAction<int>>();
		list.Add(callback);
		uploadings.Add(key, list);
		string uri = string.Format("http://{0}:{1}/", AppConst.ServerIp, AppConst.WebrServerPort);
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddBinaryData("file", data);
		UnityWebRequest unityWebRequest = UnityWebRequest.Post(uri, wWWForm);
		unityWebRequest.SetRequestHeader("cmd", cmd);
		unityWebRequest.SetRequestHeader("gameid", gameId);
		if (otherData != null)
		{
			foreach (string key2 in otherData.Keys)
			{
				unityWebRequest.SetRequestHeader(key2, otherData[key2]);
			}
		}
		StartCoroutine(UploadFileCrounte(key, unityWebRequest));
	}

	private IEnumerator UploadFileCrounte(string key, UnityWebRequest request)
	{
		yield return request.Send();
		int code = 0;
		if (request.responseCode == 200)
		{
			code = 1;
		}
		else
		{
			Util.LogWarning("upload error info :" + request.responseCode);
			code = 0;
		}
		List<UnityAction<int>> callbacks = uploadings[key];
		uploadings.Remove(key);
		for (int i = 0; i < callbacks.Count; i++)
		{
			UnityAction<int> unityAction = callbacks[i];
			if (unityAction != null)
			{
				try
				{
					unityAction.MyInvoke(code);
				}
				catch (Exception ex)
				{
					Util.LogWarning("www upload file do callback error ----->>>>" + ex.ToString());
				}
			}
		}
	}

	public void DownloadFile(string cmd, string gameId, Type typ, Dictionary<string, string> otherData = null, UnityAction<int, object> callback = null)
	{
		List<UnityAction<int, object>> list = null;
		string key = cmd + gameId + typ.ToString();
		if (downloadings.ContainsKey(key))
		{
			list = downloadings[key];
			list.Add(callback);
			return;
		}
		list = new List<UnityAction<int, object>>();
		list.Add(callback);
		downloadings.Add(key, list);
		string url = string.Format("http://{0}:{1}/", AppConst.ServerIp, AppConst.WebrServerPort);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("cmd", cmd);
		dictionary.Add("gameid", gameId);
		if (otherData != null)
		{
			foreach (string key2 in otherData.Keys)
			{
				dictionary.Add(key2, otherData[key2]);
			}
		}
		StartCoroutine(StarDownLoadFile(key, url, dictionary, typ));
	}

	private IEnumerator StarDownLoadFile(string key, string url, Dictionary<string, string> header, Type type)
	{
		WWW www = new WWW(url, null, header);
		yield return www;
		int code = 0;
		if (string.IsNullOrEmpty(www.error))
		{
			code = 1;
		}
		List<UnityAction<int, object>> callbacks = downloadings[key];
		downloadings.Remove(key);
		for (int i = 0; i < callbacks.Count; i++)
		{
			UnityAction<int, object> unityAction = callbacks[i];
			if (unityAction == null)
			{
				continue;
			}
			try
			{
				if (code == 0)
				{
					unityAction(0, null);
				}
				else if (type == typeof(Texture2D))
				{
					unityAction(1, www.texture);
				}
				else if (type == typeof(byte[]))
				{
					unityAction(1, www.bytes);
				}
			}
			catch (Exception ex)
			{
				Util.LogWarning("www download file do callback error ----->>>>" + ex.ToString());
			}
		}
	}

	public void CheckGoodsIsPurch(string palyerId, string goodsId, string appid, UnityAction<bool> callback = null)
	{
		string path = string.Format("http://50.116.26.130:9989/cmd/?cmd=checkpurch&uniqueId={0}&goodsId={1}&appId={2}", palyerId, goodsId, appid);
		StartCoroutine(CheckGoods(path, callback));
	}

	public void AddGoodsPurch(string palyerId, string goodsId, string appid, UnityAction<bool> callback = null)
	{
		string path = string.Format("http://50.116.26.130:9989/cmd/?cmd=addpurch&uniqueId={0}&goodsId={1}&appId={2}", palyerId, goodsId, appid);
		StartCoroutine(CheckGoods(path, callback));
	}

	private IEnumerator CheckGoods(string path, UnityAction<bool> callback)
	{
		WWW www = new WWW(path);
		yield return www;
		if (string.IsNullOrEmpty(www.error))
		{
			callback.MyInvoke(true);
		}
		else
		{
			callback.MyInvoke(false);
		}
	}
}
