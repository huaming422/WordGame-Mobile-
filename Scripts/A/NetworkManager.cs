using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NetworkManager : SingleObject<NetworkManager>
{
	private static readonly object lockobj = new object();

	private static Queue<KeyValuePair<int, ByteBuffer>> messages = new Queue<KeyValuePair<int, ByteBuffer>>();

	public UnityAction onConect;

	public UnityAction onDisconct;

	private Dictionary<int, List<EventItem<string>>> stringEvents = new Dictionary<int, List<EventItem<string>>>();

	private Dictionary<int, List<EventItem<byte[]>>> binaryEvents = new Dictionary<int, List<EventItem<byte[]>>>();

	private int deliverPerfarme = 10;

	private int nowDeliver;

	private static SocketClient socket;

	private static SocketClient SocketClient
	{
		get
		{
			if (socket == null)
			{
				socket = new SocketClient();
			}
			return socket;
		}
	}

	public bool connected
	{
		get
		{
			if (socket == null)
			{
				return false;
			}
			return socket.Connected;
		}
	}

	public static bool sconnected
	{
		get
		{
			if (socket == null)
			{
				return false;
			}
			return socket.Connected;
		}
	}

	private void Awake()
	{
		Init();
	}

	private void Init()
	{
		SingleSysObj<HeartBetManager>.instance.Pinit();
	}

	public void AddStringEvent(int id, UnityAction<int, string> action, UnityEngine.Object objReference = null)
	{
		EventItem<string> eventItem = null;
		eventItem = new EventItem<string>(id, action, objReference);
		if (stringEvents.ContainsKey(id))
		{
			stringEvents[id].Add(eventItem);
			return;
		}
		List<EventItem<string>> list = new List<EventItem<string>>();
		list.Add(eventItem);
		stringEvents.Add(id, list);
	}

	public void AddBinaryEvent(int id, UnityAction<int, byte[]> action, UnityEngine.Object objReference)
	{
		EventItem<byte[]> eventItem = null;
		eventItem = new EventItem<byte[]>(id, action, objReference);
		if (binaryEvents.ContainsKey(id))
		{
			binaryEvents[id].Add(eventItem);
			return;
		}
		List<EventItem<byte[]>> list = new List<EventItem<byte[]>>();
		list.Add(eventItem);
		binaryEvents.Add(id, list);
	}

	public void RemoveStringEvent(int id, UnityAction<int, string> action = null)
	{
		DoRemoveEvent(stringEvents, id, action);
	}

	public void RemoveBinaryEvent(int id, UnityAction<int, byte[]> action = null)
	{
		DoRemoveEvent(binaryEvents, id, action);
	}

	private void DoRemoveEvent<T>(Dictionary<int, List<EventItem<T>>> messageEvents, int id, UnityAction<int, T> action)
	{
		if (!messageEvents.ContainsKey(id))
		{
			return;
		}
		if (action == null)
		{
			messageEvents.Remove(id);
			return;
		}
		List<EventItem<T>> list = messageEvents[id];
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (list[num].action == action)
			{
				list.RemoveAt(num);
			}
		}
		if (list.Count == 0)
		{
			messageEvents.Remove(id);
		}
	}

	public static void AddMessage(int _event, ByteBuffer data)
	{
		lock (lockobj)
		{
			messages.Enqueue(new KeyValuePair<int, ByteBuffer>(_event, data));
		}
	}

	private void Update()
	{
		if (messages.Count <= 0)
		{
			return;
		}
		nowDeliver = 0;
		while (messages.Count > 0 && nowDeliver < deliverPerfarme)
		{
			nowDeliver++;
			KeyValuePair<int, ByteBuffer> keyValuePair = messages.Dequeue();
			if (NetWorkProtocal.isNetProtocal(keyValuePair.Key))
			{
				DeliverSocketMessage(keyValuePair.Key);
			}
			else
			{
				DeliverMessagae(keyValuePair.Key, keyValuePair.Value);
			}
		}
	}

	private void DeliverSocketMessage(int id)
	{
		if (id == 100)
		{
			OnConect();
		}
		if (id == 104)
		{
			OnDisconect();
		}
		if (id == 103)
		{
			OnException();
		}
		if (id == 101)
		{
			OnConnectError();
		}
	}

	private void DeliverMessagae(int id, ByteBuffer buffer)
	{
		int num = buffer.ReadByte();
		if (num == 0)
		{
			string text = buffer.ReadString();
			Util.LogWarning(string.Format("network  recive StringMessage {0} <<<<<------ {1}", id, text));
			if (!stringEvents.ContainsKey(id))
			{
				return;
			}
			ExecuteMessage(stringEvents, id, text);
		}
		if (num == 1 && binaryEvents.ContainsKey(id))
		{
			byte[] data = buffer.ReadBytes();
			ExecuteMessage(binaryEvents, id, data);
		}
	}

	private void ExecuteMessage<T>(Dictionary<int, List<EventItem<T>>> messageEvents, int id, T data)
	{
		List<EventItem<T>> list = messageEvents[id];
		for (int num = list.Count - 1; num >= 0; num--)
		{
			EventItem<T> eventItem = list[num];
			if (eventItem.isAutoRemove && !eventItem.reference.isAlive)
			{
				list.RemoveAt(num);
				if (list.Count == 0)
				{
					messageEvents.Remove(id);
					break;
				}
			}
			else
			{
				UnityAction<int, T> action = eventItem.action;
				try
				{
					if (action != null)
					{
						action(id, data);
					}
				}
				catch (Exception arg)
				{
					Util.LogWarning(string.Format("Networkmanager Execute Message {0} Error ----->>> {1}", id, arg));
				}
			}
		}
	}

	public void SendConnect()
	{
		SocketClient.OnRegister();
		SocketClient.SendConnect(AppConst.ServerIp, AppConst.ServerPort);
	}

	private static void SendMessage(ByteBuffer buffer)
	{
		lock (SocketClient)
		{
			SocketClient.SendMessage(buffer);
		}
	}

	public static void StaticSendStringMessage(int id, string data)
	{
		if (sconnected)
		{
			ByteBuffer byteBuffer = new ByteBuffer();
			byteBuffer.WriteShort((ushort)id);
			byteBuffer.WriteByte(0);
			byteBuffer.WriteString(data);
			SendMessage(byteBuffer);
		}
	}

	public void SendStringMessage(int id, string data)
	{
		if (CheckConnectAndReconnect())
		{
			ByteBuffer byteBuffer = new ByteBuffer();
			byteBuffer.WriteShort((ushort)id);
			byteBuffer.WriteByte(0);
			byteBuffer.WriteString(data);
			SendMessage(byteBuffer);
			Util.LogWarning(string.Format("network Send StringMessage id:{0} length success ----->>> {1}", id, data));
		}
	}

	private bool CheckConnectAndReconnect()
	{
		if (connected)
		{
			return true;
		}
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			return false;
		}
		SendConnect();
		return false;
	}

	private void OnDestroy()
	{
		Debug.Log("~NetworkManager was destroy");
		SocketClient.OnRemove();
	}

	private void OnConect()
	{
		SingleSysObj<HeartBetManager>.instance.ResetLastReciveTime();
		SingleSysObj<HeartBetManager>.instance.Start();
		Util.LogWarning(string.Format("networkMessage ---->>> conect server sucess ip:{0} port:{1}", AppConst.ServerIp, AppConst.ServerPort));
	}

	private void OnConnectError()
	{
		Util.LogWarning(string.Format("networkMessage ---->>> conect server error ip:{0} port:{1}", AppConst.ServerIp, AppConst.ServerPort));
	}

	private void OnDisconect()
	{
		SingleSysObj<HeartBetManager>.instance.StopHeartBet();
		Util.LogWarning(string.Format("networkMessage ---->>> disconect server ip:{0} port:{1}", AppConst.ServerIp, AppConst.ServerPort));
	}

	private void OnException()
	{
		SingleSysObj<HeartBetManager>.instance.StopHeartBet();
		Util.LogWarning(string.Format("networkMessage ---->>> receive exception ip:{0} port:{1}", AppConst.ServerIp, AppConst.ServerPort));
	}

	public static void Close()
	{
		if (socket != null)
		{
			socket.Close();
		}
	}
}
