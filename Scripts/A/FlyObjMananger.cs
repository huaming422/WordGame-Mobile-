using UnityEngine;
using UnityEngine.Events;

public class FlyObjMananger : MonoBehaviour
{
	public static FlyObjMananger instance;

	private void Awake()
	{
		instance = this;
	}

	public static void ShowFlyObj(GameObject prefab, Vector3 startPos, Vector3 endPos, Vector3 endScale, float time, float delay, int count, UnityAction<GameObject> perCallback, UnityAction finishcallback, Transform parent)
	{
		if (prefab == null)
		{
			return;
		}
		int nowCount = 0;
		if (parent == null)
		{
			parent = prefab.transform.parent;
		}
		SingleObject<SchudleManger>.instance.Schudle(delegate
		{
			GameObject temp = Object.Instantiate(prefab);
			temp.SetActive(true);
			if (parent != null)
			{
				temp.transform.SetParent(parent, false);
			}
			temp.transform.position = startPos;
			iTween.LinerMoveTo(temp, endPos, time);
			iTween.LinerScaleTo(temp, endScale, time);
			SingleObject<SchudleManger>.instance.Schudle(delegate
			{
				if (perCallback != null)
				{
					perCallback(temp);
				}
				if (temp != null)
				{
					Object.Destroy(temp);
				}
				nowCount++;
				if (nowCount >= count && finishcallback != null)
				{
					finishcallback();
				}
			}, time);
		}, delay, true, count);
	}

	public static void FlyObj(GameObject prefab, Vector3 endPos, float time, float delay, int count, UnityAction finishcallback, Transform parent)
	{
		if (!(prefab == null))
		{
			ShowFlyObj(prefab, prefab.transform.position, endPos, prefab.transform.localScale, time, delay, count, null, finishcallback, parent);
		}
	}

	public static void FlyObj(GameObject prefab, Vector3 endPos, float time, float delay, int count)
	{
		if (!(prefab == null))
		{
			ShowFlyObj(prefab, prefab.transform.position, endPos, prefab.transform.localScale, time, delay, count, null, null, null);
		}
	}

	public static void FlyObj(GameObject prefab, Vector3 startpos, Vector3 endPos, float time, float delay, int count, UnityAction finishcallback = null, Transform parent = null)
	{
		if (!(prefab == null))
		{
			ShowFlyObj(prefab, startpos, endPos, prefab.transform.localScale, time, delay, count, null, finishcallback, parent);
		}
	}
}
