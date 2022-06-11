using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DragEventListener : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IEventSystemHandler
{
	public UnityAction<DragEventParm> onBeginDrag;

	public UnityAction<DragEventParm> onDrag;

	public UnityAction<DragEventParm> onEndDrag;

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (onBeginDrag != null)
		{
			DragEventParm arg = new DragEventParm(base.gameObject, eventData.position);
			onBeginDrag(arg);
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (onDrag != null)
		{
			DragEventParm arg = new DragEventParm(base.gameObject, eventData.position);
			onDrag(arg);
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (onEndDrag != null)
		{
			DragEventParm arg = new DragEventParm(base.gameObject, eventData.position);
			onEndDrag(arg);
		}
	}
}
