using UnityEngine;
using UnityEngine.Events;

public class UnityInAppPurchManager : SingleObject<UnityInAppPurchManager>
{
	private IPurchManager purchManager;

	public void Init()
	{
		if (AppConst.Channel == GameChannel.Amazon)
		{
			purchManager = new AmzInAppPurchManager();
		}
		if (AppConst.Channel == GameChannel.Google)
		{
			purchManager = new GoogleInAppPurchManager();
		}
		if (purchManager == null)
		{
			Debug.Log("purchManager == null");
		}
		else
		{
			purchManager.Init();
		}
	}

	public void Purch(string id, UnityAction<bool, string> callback)
	{
		if (purchManager != null)
		{
			purchManager.Purch(id, callback);
		}
	}

	public void CheckGoodIsPurch(string id, UnityAction<bool> callback)
	{
		if (purchManager != null)
		{
			purchManager.CheckGoodsIsPurch(id, callback);
		}
	}

	public void AddGoodPurch(string id, UnityAction<bool> callback = null)
	{
		if (purchManager != null)
		{
			purchManager.AddGoodPurch(id, callback);
		}
	}
}
