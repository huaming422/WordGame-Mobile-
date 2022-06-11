using System.Collections;
using System.Collections.Generic;

public class CanSaveEventManager : SingleObject<CanSaveEventManager>
{
	private string saveDatakey = "CanSaveEventManager_DataKey";

	private Hashtable savedEvents;

	private void Awake()
	{
		ReadEventData();
	}

	private void ReadEventData()
	{
		string @string = AccountDataManager.GetString(saveDatakey, string.Empty);
		if (string.IsNullOrEmpty(@string))
		{
			savedEvents = new Hashtable();
			savedEvents["nowId"] = 0;
		}
		else
		{
			savedEvents = @string.DecodeJson();
		}
	}

	private void SaveEventData()
	{
		if (savedEvents != null)
		{
			AccountDataManager.SetString(saveDatakey, savedEvents.ToJson());
		}
	}

	private int GetNextIndex(bool isAdd = true)
	{
		int @int = savedEvents.GetInt("nowId");
		@int++;
		if (isAdd)
		{
			savedEvents["nowId"] = @int;
		}
		return @int;
	}

	private Hashtable GetAllEvents()
	{
		if (!savedEvents.ContainsKey("events"))
		{
			savedEvents["events"] = new Hashtable();
		}
		return savedEvents["events"] as Hashtable;
	}

	private ArrayList GetEventsByType(Hashtable events, int type)
	{
		string key = type.ToString();
		if (!events.ContainsKey(key))
		{
			events[key] = new ArrayList();
		}
		return events[key] as ArrayList;
	}

	public void AddEvent(int type, Hashtable data)
	{
		if (data == null)
		{
			data = new Hashtable();
		}
		if (savedEvents == null)
		{
			ReadEventData();
		}
		int nextIndex = GetNextIndex();
		Hashtable allEvents = GetAllEvents();
		ArrayList eventsByType = GetEventsByType(allEvents, type);
		Hashtable hashtable = new Hashtable();
		hashtable["id"] = nextIndex;
		hashtable["data"] = data;
		eventsByType.Add(hashtable);
		SaveEventData();
	}

	public void RemoveEvent(int type, int id)
	{
		if (savedEvents == null)
		{
			ReadEventData();
		}
		Hashtable allEvents = GetAllEvents();
		ArrayList eventsByType = GetEventsByType(allEvents, type);
		for (int i = 0; i < eventsByType.Count; i++)
		{
			Hashtable data = eventsByType[i] as Hashtable;
			int @int = data.GetInt("id");
			if (@int == id)
			{
				eventsByType.RemoveAt(i);
				SaveEventData();
				break;
			}
		}
	}

	public void RemoveEvents(int type)
	{
		if (savedEvents == null)
		{
			ReadEventData();
		}
		Hashtable allEvents = GetAllEvents();
		if (allEvents.ContainsKey(type.ToString()))
		{
			allEvents.Remove(type.ToString());
			SaveEventData();
		}
	}

	public List<CanSaveEventItem> GetEvents(int type)
	{
		if (savedEvents == null)
		{
			ReadEventData();
		}
		Hashtable allEvents = GetAllEvents();
		ArrayList eventsByType = GetEventsByType(allEvents, type);
		List<CanSaveEventItem> list = new List<CanSaveEventItem>();
		for (int i = 0; i < eventsByType.Count; i++)
		{
			Hashtable data = eventsByType[i] as Hashtable;
			int @int = data.GetInt("id");
			Hashtable hashtable = data.GetHashtable("data");
			CanSaveEventItem item = new CanSaveEventItem(@int, type, hashtable);
			list.Add(item);
		}
		return list;
	}
}
