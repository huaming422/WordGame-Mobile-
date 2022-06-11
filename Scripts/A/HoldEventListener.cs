using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoldEventListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IEventSystemHandler
{
	public float holdTimes = 2f;

	public UnityEvent onHoldEvent = new UnityEvent();

	public UnityEvent onHoldTimeEnough = new UnityEvent();

	private bool _isHanding;

	private float _nowHoldTime;

	private bool _isHandEvent;

	public void OnPointerClick(PointerEventData eventData)
	{
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		Selectable component = GetComponent<Selectable>();
		if (component != null)
		{
			component.OnPointerDown(eventData);
			if (!component.interactable)
			{
				return;
			}
		}
		_isHandEvent = false;
		_isHanding = true;
		_nowHoldTime = 0f;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		Selectable component = GetComponent<Selectable>();
		if (component != null)
		{
			component.OnPointerUp(eventData);
			if (!component.interactable)
			{
				return;
			}
		}
		_isHanding = false;
		if (_isHandEvent)
		{
			onHoldEvent.Invoke();
			return;
		}
		Button component2 = GetComponent<Button>();
		if (component != null)
		{
			component2.OnPointerClick(eventData);
		}
	}

	private void Update()
	{
		if (_isHanding)
		{
			_nowHoldTime += Time.deltaTime;
			if (_nowHoldTime > holdTimes)
			{
				_isHandEvent = true;
				_isHanding = false;
				onHoldTimeEnough.Invoke();
			}
		}
	}
}
