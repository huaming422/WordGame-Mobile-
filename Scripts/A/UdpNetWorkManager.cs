using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UdpNetWorkManager : SingleObject<UdpNetWorkManager>
{
	private MyUdpClient client;

	private Dictionary<int, UnityAction<string>> stringMesageEvent = new Dictionary<int, UnityAction<string>>();

	private float heartBeatTime = 30f;

	private float SendheartBetIntervalTime;

	private bool isCheckHeart;

	public bool networkEnable;

	private float checkNetworkRate = 60f;

	private float nowCheckNetIntervalTime;

	public UnityAction onNetworkDisconnect;

	public UnityAction onNetworkConnect;

	public void Init(string ip, int serverPort, int localPort)
	{
		InitNetworkState();
		client = new MyUdpClient();
		client.Init(ip, serverPort, localPort);
		MyUdpClient myUdpClient = client;
		myUdpClient.onReceveData = (Action<int, ByteBuffer>)Delegate.Combine(myUdpClient.onReceveData, new Action<int, ByteBuffer>(OnReveMessage));
		isCheckHeart = true;
		SendheartBetIntervalTime = heartBeatTime;
		nowCheckNetIntervalTime = checkNetworkRate;
	}

	public void AddStringEvent(int id, UnityAction<string> action)
	{
		if (!stringMesageEvent.ContainsKey(id))
		{
			stringMesageEvent.Add(id, action);
		}
	}

	public void RemoveStringEvent(int id)
	{
		if (stringMesageEvent.ContainsKey(id))
		{
			stringMesageEvent.Remove(id);
		}
	}

	private void OnReveMessage(int id, ByteBuffer buffer)
	{
		int num = buffer.ReadByte();
		if (num == 1)
		{
			string text = buffer.ReadString();
			Util.LogWarning("recive string message <<<<<<------- id:" + id + "  content:" + text);
			BoradCastStringMessage(id, text);
		}
	}

	public void BoradCastStringMessage(int id, string data)
	{
		if (stringMesageEvent.ContainsKey(id))
		{
			UnityAction<string> unityAction = stringMesageEvent[id];
			if (unityAction == null)
			{
				stringMesageEvent.Remove(id);
			}
			else
			{
				unityAction(data);
			}
		}
	}

	public void SendMessage(int id, string data)
	{
		if (client != null && networkEnable)
		{
			ByteBuffer byteBuffer = new ByteBuffer();
			byteBuffer.WriteShort((ushort)id);
			byteBuffer.WriteByte(1);
			byteBuffer.WriteString(data);
			Util.LogWarning(" ----->>>>send data to server id:" + id + " length:" + data.Length + "  content:" + data);
			client.SendMessage(byteBuffer);
		}
	}

	public void SendMessage(int id, byte[] data)
	{
		if (client != null && networkEnable)
		{
			ByteBuffer byteBuffer = new ByteBuffer();
			byteBuffer.WriteShort((ushort)id);
			byteBuffer.WriteByte(2);
			byteBuffer.WriteBytes(data);
			Util.LogWarning("----->>>>send binary data to server id:" + id + " length:" + data.Length);
			client.SendMessage(byteBuffer);
		}
	}

	private void Update()
	{
		if (client != null)
		{
			client.Update();
		}
	}

	private void OnDestroy()
	{
		if (client != null)
		{
			client.Release();
		}
	}

	private void InitNetworkState()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			networkEnable = false;
		}
		else
		{
			networkEnable = true;
		}
	}
}
