using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class SocketClient
{
	private TcpClient client;

	private NetworkStream outStream;

	private MemoryStream memStream;

	private BinaryReader reader;

	private const int MAX_READ = 8192;

	private byte[] byteBuffer = new byte[8192];

	public static bool loggedIn;

	public bool Connected
	{
		get
		{
			if (client == null)
			{
				return false;
			}
			return client.Connected;
		}
	}

	public void OnRegister()
	{
		if (reader != null)
		{
			reader.Close();
		}
		if (memStream != null)
		{
			memStream.Close();
		}
		memStream = new MemoryStream();
		reader = new BinaryReader(memStream);
	}

	public void OnRemove()
	{
		Close();
		if (reader != null && memStream != null)
		{
			reader.Close();
			memStream.Close();
		}
	}

	private void ConnectServer(string host, int port)
	{
		client = null;
		try
		{
			IPAddress[] hostAddresses = Dns.GetHostAddresses(host);
			if (hostAddresses.Length == 0)
			{
				Debug.LogError("host invalid");
				return;
			}
			if (hostAddresses[0].AddressFamily == AddressFamily.InterNetworkV6)
			{
				client = new TcpClient(AddressFamily.InterNetworkV6);
			}
			else
			{
				client = new TcpClient(AddressFamily.InterNetwork);
			}
			client.SendTimeout = 3000;
			client.ReceiveTimeout = 3000;
			client.NoDelay = true;
			client.BeginConnect(host, port, OnConnect, null);
		}
		catch (Exception ex)
		{
			Close();
			AddNetWorkMessage(101, string.Empty);
			Util.LogWarning(ex.Message);
		}
	}

	private void OnConnect(IAsyncResult asr)
	{
		try
		{
			client.EndConnect(asr);
			outStream = client.GetStream();
			client.GetStream().BeginRead(byteBuffer, 0, 8192, OnRead, null);
			NetworkManager.AddMessage(100, new ByteBuffer());
		}
		catch (Exception ex)
		{
			AddNetWorkMessage(101, string.Empty);
			Util.LogWarning(ex.Message);
		}
	}

	private void WriteMessage(byte[] message)
	{
		MemoryStream memoryStream = null;
		using (memoryStream = new MemoryStream())
		{
			memoryStream.Position = 0L;
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			ushort value = (ushort)message.Length;
			binaryWriter.Write(value);
			binaryWriter.Write(message);
			binaryWriter.Flush();
			if (client != null && client.Connected)
			{
				byte[] array = memoryStream.ToArray();
				outStream.BeginWrite(array, 0, array.Length, OnWrite, null);
			}
			else
			{
				Util.LogWarning("client.connected----->>false");
			}
		}
	}

	private void OnRead(IAsyncResult asr)
	{
		int num = 0;
		try
		{
			lock (client.GetStream())
			{
				num = client.GetStream().EndRead(asr);
			}
			if (num < 1)
			{
				OnDisconnected(DisType.Disconnect, "bytesRead < 1");
				return;
			}
			OnReceive(byteBuffer, num);
			lock (client.GetStream())
			{
				Array.Clear(byteBuffer, 0, byteBuffer.Length);
				client.GetStream().BeginRead(byteBuffer, 0, 8192, OnRead, null);
			}
		}
		catch (Exception ex)
		{
			OnDisconnected(DisType.Exception, ex.Message);
		}
	}

	private void OnDisconnected(DisType dis, string msg)
	{
		Close();
		int num = ((dis != 0) ? 104 : 103);
		ByteBuffer byteBuffer = new ByteBuffer();
		byteBuffer.WriteShort((ushort)num);
		NetworkManager.AddMessage(num, byteBuffer);
		Util.LogWarning("Connection was closed by the server:>" + msg + " Distype:>" + dis);
	}

	private void PrintBytes()
	{
		string text = string.Empty;
		for (int i = 0; i < byteBuffer.Length; i++)
		{
			text += byteBuffer[i].ToString("X2");
		}
		Debug.LogError(text);
	}

	private void OnWrite(IAsyncResult r)
	{
		try
		{
			outStream.EndWrite(r);
		}
		catch (Exception ex)
		{
			Debug.LogError("OnWrite--->>>" + ex.Message);
		}
	}

	private void OnReceive(byte[] bytes, int length)
	{
		memStream.Seek(0L, SeekOrigin.End);
		memStream.Write(bytes, 0, length);
		memStream.Seek(0L, SeekOrigin.Begin);
		while (RemainingBytes() > 2)
		{
			ushort num = reader.ReadUInt16();
			if (RemainingBytes() >= (int)num)
			{
				MemoryStream memoryStream = new MemoryStream();
				BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
				binaryWriter.Write(reader.ReadBytes(num));
				memoryStream.Seek(0L, SeekOrigin.Begin);
				OnReceivedMessage(memoryStream);
				continue;
			}
			memStream.Position -= 2;
			break;
		}
		byte[] array = reader.ReadBytes((int)RemainingBytes());
		memStream.SetLength(0L);
		memStream.Write(array, 0, array.Length);
	}

	private long RemainingBytes()
	{
		return memStream.Length - memStream.Position;
	}

	private void OnReceivedMessage(MemoryStream ms)
	{
		BinaryReader binaryReader = new BinaryReader(ms);
		byte[] data = binaryReader.ReadBytes((int)(ms.Length - ms.Position));
		ByteBuffer byteBuffer = new ByteBuffer(data);
		int @event = byteBuffer.ReadShort();
		NetworkManager.AddMessage(@event, byteBuffer);
	}

	private void SessionSend(byte[] bytes)
	{
		WriteMessage(bytes);
	}

	public void Close()
	{
		if (client != null)
		{
			if (client.Connected)
			{
				client.Close();
			}
			client = null;
		}
		loggedIn = false;
	}

	public void SendConnect(string addr, int port)
	{
		ConnectServer(addr, port);
	}

	public void SendMessage(ByteBuffer buffer)
	{
		SessionSend(buffer.ToBytes());
		buffer.Close();
	}

	private void AddNetWorkMessage(int id, string message)
	{
		ByteBuffer byteBuffer = new ByteBuffer();
		byteBuffer.WriteShort((ushort)id);
		NetworkManager.AddMessage(id, byteBuffer);
	}
}
