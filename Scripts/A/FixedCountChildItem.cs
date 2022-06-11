using UnityEngine;

public class FixedCountChildItem
{
	public int posIndex;

	public int dataIndex;

	public GameObject item;

	private Vector3 moveHori = Vector3.zero;

	private Vector3 moveVert = Vector3.zero;

	public Vector3 targetPos = Vector3.zero;

	public Vector3 starMovePos = Vector3.zero;

	public Vector3 position
	{
		get
		{
			if (item != null)
			{
				return item.transform.localPosition;
			}
			return Vector3.one;
		}
		set
		{
			if (item != null)
			{
				item.transform.localPosition = value;
			}
		}
	}

	public FixedCountChildItem(int pi, int di, GameObject obj)
	{
		posIndex = pi;
		dataIndex = di;
		item = obj;
	}

	public virtual void Move(Vector3 distance)
	{
		if (!(item == null))
		{
			item.transform.localPosition += distance;
		}
	}

	public virtual void MoveHori(float distance)
	{
		moveHori.x = distance;
		Move(moveHori);
	}

	public virtual void MoveVert(float distance)
	{
		moveVert.y = distance;
		Move(moveVert);
	}
}
