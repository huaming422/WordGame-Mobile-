using UnityEngine;

public class DragEventParm
{
	public GameObject target;

	public Vector3 position = Vector3.zero;

	public DragEventParm(GameObject obj, Vector3 pos)
	{
		target = obj;
		position = pos;
	}
}
