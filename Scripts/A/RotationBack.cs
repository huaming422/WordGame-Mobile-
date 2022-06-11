using UnityEngine;

public class RotationBack : MonoBehaviour
{
	public Vector3 rotatSpeed = new Vector3(0f, 0f, 30f);

	private void Update()
	{
		base.transform.Rotate(rotatSpeed * Time.deltaTime);
	}
}
