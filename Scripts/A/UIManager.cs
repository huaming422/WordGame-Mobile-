using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : SingleObject<UIManager>
{
	private class UIItem
	{
		public string budleName = string.Empty;

		public string prefabName = string.Empty;

		public UnityAction<GameObject> callBack;

		public Type type;

		public GameObject uiObj;

		public Transform parent;
	}

	private static Transform m_UIRoot;

	private Dictionary<string, UIItem> m_allUIItem = new Dictionary<string, UIItem>();

	private List<GameAsset> _openUiList = new List<GameAsset>();

	private List<string> _loadingList = new List<string>();

	public static Transform UIRoot
	{
		get
		{
			if (m_UIRoot == null)
			{
				m_UIRoot = GameObject.Find("Canvas").transform.Find("UIRoot");
			}
			return m_UIRoot;
		}
	}

	public GameAsset GetNowUI()
	{
		if (_openUiList.Count == 0)
		{
			return null;
		}
		return _openUiList[_openUiList.Count - 1];
	}

	public void OpenUI(GameAsset asset, Type type = null, UnityAction<GameObject> callback = null, Transform parent = null, bool isSpecilUI = false)
	{
		if (asset == null)
		{
			return;
		}
		string keyString = asset.abName + asset.assetName;
		if (_loadingList.IndexOf(keyString) != -1)
		{
			return;
		}
		UIItem item = null;
		if (m_allUIItem.TryGetValue(keyString, out item))
		{
			if (item.uiObj == null)
			{
				m_allUIItem.Remove(keyString);
			}
			else
			{
				item.uiObj.SetActive(true);
			}
		}
		else
		{
			item = new UIItem();
			item.budleName = asset.abName;
			item.prefabName = asset.assetName;
		}
		item.type = type;
		item.callBack = callback;
		item.parent = parent;
		if (item.uiObj != null)
		{
			ShowUI(item);
			return;
		}
		_loadingList.Add(keyString);
		LoadPrefab(asset.abName, asset.assetName, delegate(UnityEngine.Object obj)
		{
			_loadingList.Remove(keyString);
			if (obj as GameObject != null)
			{
				if (m_allUIItem.ContainsKey(keyString))
				{
					m_allUIItem.TryGetValue(keyString, out item);
				}
				else
				{
					item.uiObj = obj as GameObject;
					item.uiObj.transform.SetParent(UIRoot, false);
					m_allUIItem.Add(keyString, item);
				}
				ShowUI(item);
				_openUiList.Add(asset);
			}
		}, isSpecilUI);
	}

	public void OpenTempUI(GameAsset asset, UnityAction<GameObject> callback = null)
	{
		string keyString = asset.abName + asset.assetName;
		if (m_allUIItem.ContainsKey(keyString))
		{
			return;
		}
		UIItem item = new UIItem();
		m_allUIItem.Add(keyString, item);
		LoadPrefab(asset.abName, asset.assetName, delegate(UnityEngine.Object obj)
		{
			m_allUIItem.Remove(keyString);
			GameObject gameObject = obj as GameObject;
			item.uiObj = gameObject;
			if (gameObject != null)
			{
				gameObject.transform.SetParent(UIRoot, false);
				gameObject.transform.SetAsLastSibling();
				if (callback != null)
				{
					callback(gameObject);
				}
			}
		});
	}

	private void ShowUI(UIItem item)
	{
		if (item.uiObj != null)
		{
			item.uiObj.transform.SetAsLastSibling();
			if (item.type != null && item.uiObj.GetComponent(item.type) == null)
			{
				item.uiObj.AddComponent(item.type);
			}
			if (item.callBack != null)
			{
				item.callBack(item.uiObj);
			}
		}
	}

	public void Close(GameAsset asset, bool isDestroy = true, bool isDestroyAb = true)
	{
		UIItem value = null;
		string key = asset.abName + asset.assetName;
		if (m_allUIItem.TryGetValue(key, out value))
		{
			if (isDestroy)
			{
				UnityEngine.Object.DestroyImmediate(value.uiObj);
				m_allUIItem.Remove(key);
				value = null;
				int num = _openUiList.FindIndex((GameAsset temp) => temp.Equals(asset));
				if (num != -1)
				{
					_openUiList.RemoveAt(num);
				}
			}
			else
			{
				value.uiObj.SetActive(false);
			}
			if (isDestroyAb && isDestroy)
			{
				SingleObject<ResourceManager>.instance.UnloadUnUseAssetBundle();
			}
		}
		Util.ClearMemory();
	}

	public void DelayColse(GameAsset asset, float time, bool isDestroy = true, bool isDestroyAb = true)
	{
		SingleObject<SchudleManger>.instance.Schudle(delegate
		{
			Close(asset, isDestroy, isDestroyAb);
		}, time);
	}

	public void CloseAll(List<GameAsset> except = null, bool isDestroy = true, bool isDestroyAb = true)
	{
		List<string> list = new List<string>();
		foreach (string key in m_allUIItem.Keys)
		{
			if (except == null)
			{
				list.Add(key);
			}
			else if (except.FindIndex((GameAsset asst) => asst.abName + asst.assetName == key) == -1)
			{
				list.Add(key);
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			string key2 = list[i];
			UIItem uIItem = m_allUIItem[key2];
			if (isDestroy)
			{
				UnityEngine.Object.DestroyImmediate(uIItem.uiObj);
				m_allUIItem.Remove(key2);
				int num = _openUiList.FindIndex((GameAsset temp) => temp.EqualsKey(key2));
				if (num != -1)
				{
					_openUiList.RemoveAt(num);
				}
			}
			else
			{
				uIItem.uiObj.SetActive(false);
			}
		}
		if (isDestroyAb && isDestroy)
		{
			SingleObject<ResourceManager>.instance.UnloadUnUseAssetBundle();
		}
	}

	private void LoadPrefab(string abName, string assetName, Action<UnityEngine.Object> func, bool isSpecilUI = false)
	{
		if (isSpecilUI)
		{
			SingleObject<ResourceManager>.instance.LoadSpecilAsset(abName, assetName, delegate(GameObject obj)
			{
				if (func != null)
				{
					func(obj);
				}
			}, null);
			return;
		}
		SingleObject<ResourceManager>.instance.LoadAsset<GameObject>(abName, assetName, delegate(UnityEngine.Object obj)
		{
			if (func != null)
			{
				func(obj);
			}
		}, null);
	}
}
