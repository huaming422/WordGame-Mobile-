using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class ResourceManager : SingleObject<ResourceManager>
{
	private class LoadAssetRequest
	{
		public Type assetType;

		public string[] assetNames;

		public Action<UnityEngine.Object[]> sharpFunc;

		public UnityEngine.Object refObj;
	}

	private string[] m_AllManifest;

	private AssetBundleManifest m_AssetBundleManifest;

	private Dictionary<string, string[]> m_Dependencies = new Dictionary<string, string[]>();

	private Dictionary<string, AssetBundleInfo> m_LoadedAssetBundles = new Dictionary<string, AssetBundleInfo>();

	private Dictionary<string, List<LoadAssetRequest>> m_LoadRequests = new Dictionary<string, List<LoadAssetRequest>>();

	private Dictionary<string, Dictionary<string, Sprite>> _sLoadedSprite = new Dictionary<string, Dictionary<string, Sprite>>();

	public void Initialize(string manifestName, Action initOK)
	{
		if (!AppConst.BundleMode)
		{
			if (initOK != null)
			{
				initOK();
			}
			return;
		}
		LoadAsset<AssetBundleManifest>(manifestName, new string[1] { "AssetBundleManifest" }, delegate(UnityEngine.Object[] objs)
		{
			if (objs.Length > 0)
			{
				m_AssetBundleManifest = objs[0] as AssetBundleManifest;
				m_AllManifest = m_AssetBundleManifest.GetAllAssetBundles();
			}
			if (initOK != null)
			{
				initOK();
			}
		}, null);
	}

	public void SetAssetBundlePermanent(GameAsset asset, bool isPermanent = true)
	{
		if (asset != null)
		{
			SetAssetBundlePermanent(asset.abName, isPermanent);
		}
	}

	public void SetAssetBundlePermanent(string abName, bool isPermanent = true)
	{
		if (AppConst.BundleMode)
		{
			abName = BundleLoadAbName(abName);
			abName = string.Join("_", abName.Split('/'));
			if (m_LoadedAssetBundles.ContainsKey(abName))
			{
				m_LoadedAssetBundles[abName].isPermanent = isPermanent;
			}
		}
	}

	private static string ResourcesLoadAbName(string abName)
	{
		if (string.IsNullOrEmpty(abName))
		{
			return string.Empty;
		}
		if (abName[abName.Length - 1] == '/')
		{
			return abName.Substring(0, abName.Length - 1);
		}
		int num = abName.LastIndexOf('/');
		if (num == -1)
		{
			return string.Empty;
		}
		return abName.Substring(0, num);
	}

	private static string BundleLoadAbName(string abName)
	{
		if (string.IsNullOrEmpty(abName))
		{
			return string.Empty;
		}
		if (abName[abName.Length - 1] == '/')
		{
			return abName.Substring(0, abName.Length - 1);
		}
		return abName;
	}

	private static T ResourcesLoadAsset<T>(string abName, string assetName) where T : UnityEngine.Object
	{
		if (assetName.LastIndexOf("/") != -1)
		{
			int num = assetName.LastIndexOf("/");
			string text = assetName.Substring(num + 1);
			abName = abName + "/" + assetName.Substring(0, num);
			T[] array = Resources.LoadAll<T>(abName);
			if (array == null || array.Length == 0)
			{
				return (T)null;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].name == text)
				{
					return array[i];
				}
			}
			return (T)null;
		}
		if (typeof(T) == typeof(GameObject))
		{
			GameObject gameObject = Resources.Load<T>(abName + "/" + assetName) as GameObject;
			if (gameObject == null)
			{
				return (T)null;
			}
			GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
			gameObject2.name = gameObject.name;
			return gameObject2 as T;
		}
		return Resources.Load<T>(abName + "/" + assetName);
	}

	private static UnityEngine.Object BundleInfoLoadAsset(AssetBundleInfo bundleInfo, string assetName, Type type, UnityEngine.Object refObj)
	{
		if (bundleInfo == null)
		{
			return null;
		}
		AssetBundle assetBundle = bundleInfo.assetBundle;
		if (assetBundle == null)
		{
			return null;
		}
		UnityEngine.Object @object = null;
		if (assetName.LastIndexOf("/") != -1)
		{
			int num = assetName.LastIndexOf("/");
			string text = assetName.Substring(num + 1);
			string text2 = assetName.Substring(0, num);
			UnityEngine.Object[] array = assetBundle.LoadAllAssets(type);
			if (array == null || array.Length == 0)
			{
				return null;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].name == text)
				{
					Sprite sprite = array[i] as Sprite;
					if (sprite != null && sprite.texture.name == text2)
					{
						@object = array[i];
						break;
					}
				}
			}
		}
		else
		{
			@object = assetBundle.LoadAsset(assetName, type);
		}
		if (@object != null)
		{
			if (type == typeof(GameObject))
			{
				UnityEngine.Object object2 = UnityEngine.Object.Instantiate(@object);
				object2.name = @object.name;
				if (refObj != null)
				{
					bundleInfo.AddAssetRenference(@object, refObj);
				}
				else
				{
					bundleInfo.AddAssetRenference(@object, object2);
				}
				@object = object2;
			}
			else
			{
				bundleInfo.AddAssetRenference(@object, refObj);
			}
		}
		return @object;
	}

	public T LoadAsset<T>(GameAsset asset, UnityEngine.Object refObj = null) where T : UnityEngine.Object
	{
		if (asset == null)
		{
			return (T)null;
		}
		return LoadAsset<T>(asset.abName, asset.assetName, refObj);
	}

	public T LoadAsset<T>(string abName, string assetName, UnityEngine.Object refObj = null) where T : UnityEngine.Object
	{
		if (string.IsNullOrEmpty(abName) || string.IsNullOrEmpty(assetName))
		{
			return (T)null;
		}
		if (AppConst.BundleMode)
		{
			abName = BundleLoadAbName(abName);
			abName = string.Join("_", abName.Split('/'));
			T[] array = SYNCLoadAsset<T>(abName, new string[1] { assetName }, refObj);
			if (array == null || array.Length == 0)
			{
				return (T)null;
			}
			return array[0];
		}
		abName = ResourcesLoadAbName(abName);
		return ResourcesLoadAsset<T>(abName, assetName);
	}

	private T[] SYNCLoadAsset<T>(string bundleName, string[] assetnames, UnityEngine.Object refObj) where T : UnityEngine.Object
	{
		bundleName = GetRealAssetPath(bundleName);
		if (string.IsNullOrEmpty(bundleName))
		{
			return null;
		}
		List<T> list = new List<T>();
		AssetBundleInfo value = null;
		if (!m_LoadedAssetBundles.TryGetValue(bundleName, out value))
		{
			AssetBundle assetBundle = SYNCLoadBunlde(bundleName);
			if (assetBundle == null)
			{
				return null;
			}
			value = new AssetBundleInfo(assetBundle);
			m_LoadedAssetBundles.Add(bundleName, value);
		}
		if (assetnames == null || assetnames.Length == 0)
		{
			return null;
		}
		for (int i = 0; i < assetnames.Length; i++)
		{
			UnityEngine.Object @object = BundleInfoLoadAsset(value, assetnames[i], typeof(T), refObj);
			if (@object != null)
			{
				list.Add(@object as T);
			}
		}
		return list.ToArray();
	}

	private AssetBundle SYNCLoadBunlde(string bundleName)
	{
		string[] value = null;
		if (!m_Dependencies.TryGetValue(bundleName, out value))
		{
			value = m_AssetBundleManifest.GetAllDependencies(bundleName);
			m_Dependencies.Add(bundleName, value);
		}
		if (value != null)
		{
			for (int i = 0; i < value.Length; i++)
			{
				if (m_LoadedAssetBundles.ContainsKey(value[i]))
				{
					m_LoadedAssetBundles[value[i]].referencedCount++;
					continue;
				}
				AssetBundle assetBundle = SYNCLoadBunlde(value[i]);
				if (!(assetBundle == null))
				{
					AssetBundleInfo assetBundleInfo = new AssetBundleInfo(assetBundle);
					assetBundleInfo.referencedCount++;
					m_LoadedAssetBundles.Add(value[i], assetBundleInfo);
				}
			}
		}
		string path = AppConst.ResDir + bundleName;
		if (!File.Exists(path))
		{
			return null;
		}
		return AssetBundle.LoadFromMemory(File.ReadAllBytes(path));
	}

	public void LoadAsset<T>(GameAsset asset, Action<UnityEngine.Object> func, UnityEngine.Object refObj) where T : UnityEngine.Object
	{
		if (asset == null)
		{
			if (func != null)
			{
				func(null);
			}
		}
		else
		{
			LoadAsset<T>(asset.abName, asset.assetName, func, refObj);
		}
	}

	public void LoadAsset<T>(string abName, string assetName, Action<UnityEngine.Object> func, UnityEngine.Object refObj) where T : UnityEngine.Object
	{
		if (string.IsNullOrEmpty(abName) || string.IsNullOrEmpty(assetName))
		{
			return;
		}
		if (AppConst.BundleMode)
		{
			abName = BundleLoadAbName(abName);
			abName = string.Join("_", abName.Split('/'));
			LoadAsset<T>(abName, new string[1] { assetName }, delegate(UnityEngine.Object[] objs)
			{
				if (func != null)
				{
					if (objs != null && objs.Length > 0)
					{
						func(objs[0]);
					}
					else
					{
						func(null);
					}
				}
			}, refObj);
		}
		else
		{
			abName = ResourcesLoadAbName(abName);
			T obj = ResourcesLoadAsset<T>(abName, assetName);
			if (func != null)
			{
				func(obj);
			}
		}
	}

	private string GetRealAssetPath(string abName)
	{
		if (abName.Equals(AppConst.AssetDir))
		{
			return abName;
		}
		abName = abName.ToLower();
		if (!abName.EndsWith(AppConst.ExtName))
		{
			abName += AppConst.ExtName;
		}
		if (abName.Contains("/"))
		{
			return abName;
		}
		for (int i = 0; i < m_AllManifest.Length; i++)
		{
			int num = m_AllManifest[i].LastIndexOf('/');
			string text = m_AllManifest[i].Remove(0, num + 1);
			if (text.Equals(abName))
			{
				return m_AllManifest[i];
			}
		}
		Util.LogError("GetRealAssetPath Error:>>" + abName);
		return null;
	}

	private void LoadAsset<T>(string abName, string[] assetNames, Action<UnityEngine.Object[]> action, UnityEngine.Object refObj) where T : UnityEngine.Object
	{
		abName = GetRealAssetPath(abName);
		if (!string.IsNullOrEmpty(abName))
		{
			LoadAssetRequest loadAssetRequest = new LoadAssetRequest();
			loadAssetRequest.assetType = typeof(T);
			loadAssetRequest.assetNames = assetNames;
			loadAssetRequest.sharpFunc = action;
			loadAssetRequest.refObj = refObj;
			List<LoadAssetRequest> value = null;
			if (!m_LoadRequests.TryGetValue(abName, out value))
			{
				value = new List<LoadAssetRequest>();
				value.Add(loadAssetRequest);
				m_LoadRequests.Add(abName, value);
				StartCoroutine(OnLoadAsset<T>(abName));
			}
			else
			{
				value.Add(loadAssetRequest);
			}
		}
	}

	private IEnumerator OnLoadAsset<T>(string abName) where T : UnityEngine.Object
	{
		AssetBundleInfo bundleInfo = GetLoadedAssetBundle(abName);
		if (bundleInfo == null)
		{
			yield return StartCoroutine(OnLoadAssetBundle(abName, typeof(T)));
			bundleInfo = GetLoadedAssetBundle(abName);
			if (bundleInfo == null)
			{
				m_LoadRequests.Remove(abName);
				Util.LogError("OnLoadAsset--->>>" + abName);
				yield break;
			}
		}
		List<LoadAssetRequest> list = null;
		if (!m_LoadRequests.TryGetValue(abName, out list))
		{
			m_LoadRequests.Remove(abName);
			yield break;
		}
		for (int i = 0; i < list.Count; i++)
		{
			string[] assetNames = list[i].assetNames;
			List<UnityEngine.Object> list2 = new List<UnityEngine.Object>();
			foreach (string assetName in assetNames)
			{
				UnityEngine.Object item = BundleInfoLoadAsset(bundleInfo, assetName, list[i].assetType, list[i].refObj);
				list2.Add(item);
			}
			if (list[i].sharpFunc != null)
			{
				list[i].sharpFunc(list2.ToArray());
				list[i].sharpFunc = null;
			}
		}
		m_LoadRequests.Remove(abName);
	}

	private IEnumerator OnLoadAssetBundle(string abName, Type type)
	{
		string url = AppConst.ResDir + abName;
		url = Util.GetFilePathToWWWPath(url);
		WWW download2 = null;
		if (type == typeof(AssetBundleManifest))
		{
			download2 = new WWW(url);
		}
		else
		{
			string[] dependencies = m_AssetBundleManifest.GetAllDependencies(abName);
			if (dependencies.Length > 0)
			{
				if (!m_Dependencies.ContainsKey(abName))
				{
					m_Dependencies.Add(abName, dependencies);
				}
				foreach (string depName in dependencies)
				{
					AssetBundleInfo bundleInfo = null;
					if (m_LoadedAssetBundles.TryGetValue(depName, out bundleInfo))
					{
						bundleInfo.referencedCount++;
					}
					else if (!m_LoadRequests.ContainsKey(depName))
					{
						yield return StartCoroutine(OnLoadAssetBundle(depName, type));
					}
				}
			}
			download2 = WWW.LoadFromCacheOrDownload(url, m_AssetBundleManifest.GetAssetBundleHash(abName), 0u);
		}
		yield return download2;
		if (!string.IsNullOrEmpty(download2.error))
		{
			Util.LogWarning("OnLoadAsset error--->>>" + download2.error);
			yield break;
		}
		AssetBundle assetObj = download2.assetBundle;
		if (assetObj != null)
		{
			m_LoadedAssetBundles.Add(abName, new AssetBundleInfo(assetObj));
		}
	}

	private AssetBundleInfo GetLoadedAssetBundle(string abName)
	{
		AssetBundleInfo value = null;
		m_LoadedAssetBundles.TryGetValue(abName, out value);
		return value;
	}

	public void UnloadUnUseAssetBundle(bool isThorough = false)
	{
		if (!AppConst.BundleMode)
		{
			return;
		}
		List<string> list = new List<string>();
		foreach (string key in m_LoadedAssetBundles.Keys)
		{
			list.Add(key);
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (!m_LoadedAssetBundles.ContainsKey(list[i]))
			{
				continue;
			}
			string text = list[i];
			AssetBundleInfo assetBundleInfo = m_LoadedAssetBundles[text];
			if (assetBundleInfo.isDirectLoad)
			{
				assetBundleInfo.RemoveUnUseAsset();
				if (!assetBundleInfo.isPermanent && !assetBundleInfo.AssetHaveRenference())
				{
					UpdateDependencies(text, m_LoadedAssetBundles[text], isThorough);
				}
			}
		}
	}

	private void UpdateDependencies(string abName, AssetBundleInfo info, bool isThorough, int deep = 0)
	{
		string[] value = null;
		if (m_Dependencies.TryGetValue(abName, out value) && (value != null || value.Length > 0))
		{
			for (int i = 0; i < value.Length; i++)
			{
				deep++;
				if (deep > 5)
				{
					return;
				}
				string abName2 = value[i];
				AssetBundleInfo loadedAssetBundle = GetLoadedAssetBundle(abName2);
				if (loadedAssetBundle != null)
				{
					loadedAssetBundle.referencedCount--;
					UpdateDependencies(abName2, loadedAssetBundle, isThorough, deep);
				}
			}
		}
		if (info.CanDestroy())
		{
			UnloadAssetBundleInternal(abName, info.assetBundle, isThorough);
		}
	}

	private void UnloadAssetBundleInternal(string abName, AssetBundle bundle, bool isThorough)
	{
		if (!(bundle == null) && !m_LoadRequests.ContainsKey(abName))
		{
			bundle.Unload(isThorough);
			m_LoadedAssetBundles.Remove(abName);
			Util.Log(string.Format("unloadAssetBundle---->> {0} <<----- sucess. now bundleCount:{1}", abName, m_LoadedAssetBundles.Count));
		}
	}

	public Sprite LoadMulitSprite(string abName, string atlasName, string spriteName, UnityEngine.Object refObj)
	{
		if (string.IsNullOrEmpty(abName) || string.IsNullOrEmpty(atlasName) || string.IsNullOrEmpty(spriteName))
		{
			return null;
		}
		Dictionary<string, Sprite> dictionary = null;
		string text = atlasName + "/" + spriteName;
		if (_sLoadedSprite.ContainsKey(abName))
		{
			dictionary = _sLoadedSprite[abName];
			if (dictionary.ContainsKey(text))
			{
				Sprite sprite = dictionary[text];
				if (sprite != null)
				{
					return sprite;
				}
				dictionary.Remove(text);
			}
		}
		Sprite sprite2 = LoadAsset<Sprite>(abName, text, refObj);
		if (sprite2 == null)
		{
			return null;
		}
		if (dictionary == null)
		{
			dictionary = new Dictionary<string, Sprite>();
		}
		dictionary.Add(text, sprite2);
		if (!_sLoadedSprite.ContainsKey(abName))
		{
			_sLoadedSprite.Add(abName, dictionary);
		}
		return sprite2;
	}

	public void ClearnLoadSprites(string abName = "", string atlasName = "", string spriteName = "")
	{
		if (string.IsNullOrEmpty(abName))
		{
			_sLoadedSprite.Clear();
			Util.ClearMemory();
		}
		else if (string.IsNullOrEmpty(atlasName))
		{
			if (_sLoadedSprite.ContainsKey(abName))
			{
				_sLoadedSprite[abName].Clear();
				_sLoadedSprite.Remove(abName);
				Util.ClearMemory();
			}
		}
		else if (!string.IsNullOrEmpty(atlasName) && !string.IsNullOrEmpty(spriteName))
		{
			string key = atlasName + "/" + spriteName;
			if (_sLoadedSprite.ContainsKey(abName) && _sLoadedSprite[abName].ContainsKey(key))
			{
				_sLoadedSprite[abName].Remove(key);
				Util.ClearMemory();
			}
		}
	}

	private static Sprite GetSpriteByName(Sprite[] sprites, string name)
	{
		for (int i = 0; i < sprites.Length; i++)
		{
			if (sprites[i].name == name)
			{
				return sprites[i];
			}
		}
		return null;
	}

	public void LoadSpecilAsset<T>(string bundleName, string assetName, UnityAction<T> callback, UnityEngine.Object refObj) where T : UnityEngine.Object
	{
		if (string.IsNullOrEmpty(bundleName) || string.IsNullOrEmpty(assetName))
		{
			if (callback != null)
			{
				callback((T)null);
			}
		}
		else if (AppConst.BundleMode)
		{
			bundleName = BundleLoadAbName(bundleName);
			bundleName = string.Join("_", bundleName.Split('/'));
			bundleName = bundleName.ToLower() + AppConst.ExtName;
			if (m_LoadedAssetBundles.ContainsKey(bundleName))
			{
				AssetBundleInfo assetBundleInfo = m_LoadedAssetBundles[bundleName];
				assetBundleInfo.referencedCount++;
				UnityEngine.Object @object = BundleInfoLoadAsset(assetBundleInfo, assetName, typeof(T), refObj);
				if (callback != null)
				{
					callback(@object as T);
				}
			}
			else
			{
				StartCoroutine(SpecilLoadBundle(bundleName, assetName, callback, refObj));
			}
		}
		else
		{
			bundleName = ResourcesLoadAbName(bundleName);
			T arg = ResourcesLoadAsset<T>(bundleName, assetName);
			if (callback != null)
			{
				callback(arg);
			}
		}
	}

	private IEnumerator SpecilLoadBundle<T>(string bundleName, string assetName, UnityAction<T> callback, UnityEngine.Object refObj) where T : UnityEngine.Object
	{
		string bundlepath2 = AppConst.ResDir + bundleName;
		WWW www2 = new WWW(Util.GetFilePathToWWWPath(bundlepath2));
		yield return www2;
		if (string.IsNullOrEmpty(www2.error))
		{
			DoLoadBundle(bundleName, www2.assetBundle, assetName, callback, refObj);
			yield break;
		}
		bundlepath2 = Util.streamingAssetsPath + bundleName;
		www2 = new WWW(Util.GetFilePathToWWWPath(bundlepath2));
		yield return www2;
		if (string.IsNullOrEmpty(www2.error))
		{
			DoLoadBundle(bundleName, www2.assetBundle, assetName, callback, refObj);
		}
		else if (callback != null)
		{
			callback((T)null);
		}
	}

	private void DoLoadBundle<T>(string bundleName, AssetBundle bundle, string assetName, UnityAction<T> callback, UnityEngine.Object refObj) where T : UnityEngine.Object
	{
		if (bundle == null && callback != null)
		{
			callback((T)null);
		}
		AssetBundleInfo assetBundleInfo = null;
		if (!m_LoadedAssetBundles.ContainsKey(bundleName))
		{
			assetBundleInfo = new AssetBundleInfo(bundle);
			assetBundleInfo.referencedCount = 1;
			m_LoadedAssetBundles.Add(bundleName, assetBundleInfo);
		}
		else
		{
			assetBundleInfo = m_LoadedAssetBundles[bundleName];
			assetBundleInfo.referencedCount++;
		}
		UnityEngine.Object @object = BundleInfoLoadAsset(assetBundleInfo, assetName, typeof(T), refObj);
		if (callback != null)
		{
			callback(@object as T);
		}
	}
}
