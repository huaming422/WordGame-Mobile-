using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PointerDonwUpListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
{
	public bool interactable = true;

	public UnityEvent onPointerDown = new UnityEvent();

	public UnityEvent onPoinerUp = new UnityEvent();

	public void OnPointerDown(PointerEventData eventData)
	{
		if (interactable)
		{
			onPointerDown.Invoke();
			PointerInOutListener pointerInOutListener = CheckHavePointerInOutListener(eventData);
			if (pointerInOutListener != null)
			{
				pointerInOutListener.OnPointerEnter(eventData);
			}
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (interactable)
		{
			onPoinerUp.Invoke();
		}
	}

	private PointerInOutListener CheckHavePointerInOutListener(PointerEventData eventData)
	{
		List<RaycastResult> list = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventData, list);
		if (list.Count == 0)
		{
			return null;
		}
		return list[0].gameObject.GetComponent<PointerInOutListener>();
	}
}
