using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PointerMoveListener : MonoBehaviour, IDragHandler, IEventSystemHandler
{
	public class MyPoinerMoveEvent : UnityEvent<Vector3>
	{
	}

	public bool interactable = true;

	public MyPoinerMoveEvent onPointerMove = new MyPoinerMoveEvent();

	private Camera _camera;

	private Vector3 _tempPos = new Vector3(0f, 0f, 0f);

	private void Start()
	{
		_camera = Camera.main;
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (interactable)
		{
			if (_camera == null)
			{
				_camera = Camera.main;
			}
			if (!(_camera == null))
			{
				_tempPos = eventData.position;
				_tempPos.z = 1f;
				onPointerMove.Invoke(_camera.ScreenToWorldPoint(_tempPos));
			}
		}
	}
}
