using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PointerInOutListener : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	public bool interactable = true;

	public UnityEvent onPointerIn = new UnityEvent();

	public UnityEvent onPoinerOut = new UnityEvent();

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (interactable)
		{
			onPointerIn.Invoke();
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (interactable)
		{
			onPoinerOut.Invoke();
		}
	}
}
