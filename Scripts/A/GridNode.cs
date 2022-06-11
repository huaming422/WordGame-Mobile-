using UnityEngine;

public class GridNode
{
	public Vector3 pos = Vector3.zero;

	public int horiPosIndex;

	public int vertPosIndex;

	public int dataIndex;

	public float width;

	public float height;

	public Vector3 leup = Vector3.zero;

	public Vector3 reup = Vector3.zero;

	public Vector3 ledow = Vector3.zero;

	public Vector3 redow = Vector3.zero;

	public void UpdateBunods()
	{
		leup.Set(pos.x - width / 2f, pos.y + height / 2f, pos.z);
		reup.Set(pos.x + width / 2f, pos.y + height / 2f, pos.z);
		ledow.Set(pos.x - width / 2f, pos.y - height / 2f, pos.z);
		redow.Set(pos.x + width / 2f, pos.y - height / 2f, pos.z);
	}
}
