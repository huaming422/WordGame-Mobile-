using UnityEngine.Events;

public interface IPurchManager
{
	void Init();

	void Purch(string id, UnityAction<bool, string> callback);

	void CheckGoodsIsPurch(string id, UnityAction<bool> callback);

	void AddGoodPurch(string id, UnityAction<bool> callback);
}
