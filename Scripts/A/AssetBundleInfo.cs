using System.Collections.Generic;
using UnityEngine;

public class AssetBundleInfo
{
	public AssetBundle assetBundle;

	public int referencedCount;

	private Dictionary<int, List<MyWeakReference>> assetRenfercenceMap;

	public bool isPermanent;

	public bool isDirectLoad;

	public AssetBundleInfo(AssetBundle assetBundle)
	{
		this.assetBundle = assetBundle;
		referencedCount = 0;
		assetRenfercenceMap = new Dictionary<int, List<MyWeakReference>>();
	}

	public void AddAssetRenference(Object asset, Object obj)
	{
		if (asset == null)
		{
			return;
		}
		isDirectLoad = true;
		if (obj == null)
		{
			isPermanent = true;
		}
		if (!isPermanent)
		{
			int hashCode = asset.GetHashCode();
			List<MyWeakReference> list = GetRenfercenceObjs(hashCode);
			if (list == null)
			{
				list = new List<MyWeakReference>();
			}
			int num = list.FindIndex((MyWeakReference weak) => weak.Target as Object == obj);
			if (num == -1)
			{
				MyWeakReference item = new MyWeakReference(obj);
				list.Add(item);
			}
			if (!assetRenfercenceMap.ContainsKey(hashCode))
			{
				assetRenfercenceMap.Add(hashCode, list);
			}
		}
	}

	public List<MyWeakReference> GetRenfercenceObjs(int assetHashCode)
	{
		if (assetRenfercenceMap.ContainsKey(assetHashCode))
		{
			return assetRenfercenceMap[assetHashCode];
		}
		return null;
	}

	public void RemoveUnUseAsset()
	{
		List<int> list = new List<int>();
		foreach (int key in assetRenfercenceMap.Keys)
		{
			List<MyWeakReference> list2 = assetRenfercenceMap[key];
			for (int num = list2.Count - 1; num >= 0; num--)
			{
				if (!list2[num].isAlive)
				{
					list2.RemoveAt(num);
				}
			}
			if (list2.Count == 0)
			{
				list.Add(key);
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			assetRenfercenceMap.Remove(list[i]);
		}
	}

	public bool CanDestroy(bool isReomveUnUseAsset = false)
	{
		if (isPermanent)
		{
			return false;
		}
		if (isReomveUnUseAsset)
		{
			RemoveUnUseAsset();
		}
		return assetRenfercenceMap.Count == 0 && referencedCount <= 0;
	}

	public bool AssetHaveRenference()
	{
		return assetRenfercenceMap.Count != 0;
	}
}
