using System;
using System.Collections;
using UnityEngine;

public class TouristGuideManager
{
	private static string _touristGuidePrefabsPath = "Prefabs/TouristGuide/";

	public static int jumpTouristGuideIndex = 10000;

	private static string _touritstGuideDataKey = "touritstGuideData";

	private static Hashtable _touritstGuideData;

	public static void TouristGuide(int index, ITouristGuide UIObj)
	{
		if (GetTouristGuideState(jumpTouristGuideIndex) != 1 && GetTouristGuideState(index) != 1)
		{
			GameAsset temp = new GameAsset(_touristGuidePrefabsPath, index.ToString());
			SingleObject<UIManager>.instance.OpenUI(temp, null, delegate(GameObject obj)
			{
				TouristGuideUICtrl component = obj.GetComponent<TouristGuideUICtrl>();
				component.myToutistUIAsset = temp;
				component.touristGuideUI = UIObj;
				component.nowTouristGuideIndex = index;
			});
		}
	}

	public static int GetTouristGuideState(int index)
	{
		if (_touritstGuideData == null)
		{
			string @string = AccountDataManager.GetString(_touritstGuideDataKey, string.Empty);
			if (string.IsNullOrEmpty(@string))
			{
				_touritstGuideData = new Hashtable();
			}
			_touritstGuideData = @string.DecodeJson();
			if (_touritstGuideData == null)
			{
				_touritstGuideData = new Hashtable();
			}
		}
		if (index != jumpTouristGuideIndex && GetTouristGuideState(jumpTouristGuideIndex) == 1)
		{
			return 1;
		}
		string key = index.ToString();
		if (_touritstGuideData.ContainsKey(key))
		{
			return Convert.ToInt32(_touritstGuideData[key]);
		}
		return -1;
	}

	public static void SetTouristGuideState(int index, int state)
	{
		if (_touritstGuideData == null)
		{
			string @string = AccountDataManager.GetString(_touritstGuideDataKey, string.Empty);
			if (string.IsNullOrEmpty(@string))
			{
				_touritstGuideData = new Hashtable();
			}
			_touritstGuideData = @string.DecodeJson();
			if (_touritstGuideData == null)
			{
				_touritstGuideData = new Hashtable();
			}
		}
		string key = index.ToString();
		_touritstGuideData[key] = state;
		string value = _touritstGuideData.ToJson();
		AccountDataManager.SetString(_touritstGuideDataKey, value);
	}
}
