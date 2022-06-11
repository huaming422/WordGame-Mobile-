using System;
using System.Collections.Generic;
using UnityEngine.Events;

public class MessageManger : SingleObject<MessageManger>
{
	private class MessageItem
	{
		public int id;

		public object message;

		public MessageItem(int id, object message)
		{
			this.id = id;
			this.message = message;
		}
	}

	private class EventItem
	{
		public int eventId;

		public UnityAction<int, object> action;

		public MyWeakReference reference;

		public bool isAutoRemove;

		public EventItem(int eventId, UnityAction<int, object> action)
		{
			this.eventId = eventId;
			this.action = action;
		}

		public EventItem(int eventId, UnityAction<int, object> action, UnityEngine.Object objReference)
		{
			this.eventId = eventId;
			this.action = action;
			if (!(objReference == null))
			{
				reference = new MyWeakReference(objReference);
				isAutoRemove = true;
			}
		}
	}

	private static Queue<MessageItem> messagesQuenue = new Queue<MessageItem>();

	private int perFrameDealMessageCount = 10;

	private int nowExecuteCount;

	private Dictionary<int, List<EventItem>> messageEvents = new Dictionary<int, List<EventItem>>();

	public void AddEvent(int id, UnityAction<int, object> action, UnityEngine.Object objReference = null)
	{
		EventItem eventItem = null;
		eventItem = ((!(objReference == null)) ? new EventItem(id, action, objReference) : new EventItem(id, action));
		if (messageEvents.ContainsKey(id))
		{
			messageEvents[id].Add(eventItem);
			return;
		}
		List<EventItem> list = new List<EventItem>();
		list.Add(eventItem);
		messageEvents.Add(id, list);
	}

	public void RemoveEvent(int eventId, UnityAction<int, object> action = null)
	{
		if (action == null)
		{
			if (messageEvents.ContainsKey(eventId))
			{
				messageEvents.Remove(eventId);
			}
		}
		else
		{
			if (!messageEvents.ContainsKey(eventId))
			{
				return;
			}
			List<EventItem> list = messageEvents[eventId];
			for (int num = list.Count - 1; num >= 0; num--)
			{
				if (list[num].action == action)
				{
					list.RemoveAt(num);
				}
			}
			if (list.Count == 0)
			{
				messageEvents.Remove(eventId);
			}
		}
	}

	private void Update()
	{
		if (messagesQuenue.Count != 0)
		{
			nowExecuteCount = 0;
			while (nowExecuteCount < perFrameDealMessageCount)
			{
				nowExecuteCount++;
				MessageItem item = FetchMessage();
				ExecuteMessage(item);
			}
		}
	}

	private void ExecuteMessage(MessageItem item)
	{
		if (item == null || !messageEvents.ContainsKey(item.id))
		{
			return;
		}
		List<EventItem> list = messageEvents[item.id];
		for (int num = list.Count - 1; num >= 0; num--)
		{
			EventItem eventItem = list[num];
			if (eventItem.isAutoRemove && !eventItem.reference.isAlive)
			{
				list.RemoveAt(num);
				if (list.Count == 0)
				{
					messageEvents.Remove(item.id);
					break;
				}
			}
			else
			{
				LogManger.Log("execute a local Message ---->>> " + item.id);
				UnityAction<int, object> action = eventItem.action;
				try
				{
					if (action != null)
					{
						action(item.id, item.message);
					}
				}
				catch (Exception arg)
				{
					Util.LogWarning(string.Format("Execute Message {0} Error ----->>>: {1}", item.id, arg));
				}
			}
		}
	}

	private static MessageItem FetchMessage()
	{
		MessageItem result = null;
		lock (messagesQuenue)
		{
			if (messagesQuenue.Count > 0)
			{
				return messagesQuenue.Dequeue();
			}
			return result;
		}
	}

	public static void ClearnMessage()
	{
		lock (messagesQuenue)
		{
			messagesQuenue.Clear();
		}
	}

	public static void SendMessage(int id, object message = null)
	{
		MessageItem item = new MessageItem(id, message);
		lock (messagesQuenue)
		{
			messagesQuenue.Enqueue(item);
		}
		LogManger.Log("recive a local Message <<<<<<------ " + id);
	}
}
