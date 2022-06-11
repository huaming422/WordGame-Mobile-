using UnityEngine;

public interface IFixCountScrollDataCtrl
{
	bool CanCreateChild(int index);

	void CreateChildItem(int index, GameObject obj);

	bool CanUpdateChildItemDataIndex(int index);

	void UpdateChildItemDataIndex(int index, GameObject obj);

	void UpdateChildItemPosIndex(int posIndex, GameObject obj);
}
