using UnityEngine;
using UnityEngine.Events;

internal class EventItem<T>
{
	public int eventId;

	public UnityAction<int, T> action;

	public MyWeakReference reference;

	public bool isAutoRemove;

	public EventItem(int eventId, UnityAction<int, T> action, Object objReference)
	{
		this.eventId = eventId;
		this.action = action;
		if (objReference != null)
		{
			isAutoRemove = true;
			reference = new MyWeakReference(objReference);
		}
	}
}
