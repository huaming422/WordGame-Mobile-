using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class MyUdpClient
{
	private class RecevePkg
	{
		public IPEndPoint romte;

		public byte[] data;

		public RecevePkg(IPEndPoint ip, byte[] data)
		{
			romte = ip;
			this.data = data;
		}
	}

	public Action<int, ByteBuffer> onReceveData;

	private UdpClient udpClient;

	private IPEndPoint server;

	private Thread readThread;

	private Queue<RecevePkg> receveMessageQue;

	public void Init(string ip, int serverPort, int loclProt)
	{
		server = new IPEndPoint(IPAddress.Parse(ip), serverPort);
		udpClient = new UdpClient(loclProt);
		receveMessageQue = new Queue<RecevePkg>();
		readThread = new Thread(ReveThread);
		readThread.IsBackground = true;
		readThread.Start();
	}

	public void SendMessage(ByteBuffer buffer)
	{
		if (buffer != null)
		{
			byte[] array = buffer.ToBytes();
			if (array != null && array.Length != 0)
			{
				udpClient.Send(array, array.Length, server);
			}
		}
	}

	private void ReveThread()
	{
		while (true)
		{
			try
			{
				while (udpClient.Available > 0)
				{
					IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
					byte[] array = udpClient.Receive(ref remoteEP);
					if (array == null || array.Length < 2)
					{
						Thread.Sleep(100);
						continue;
					}
					RecevePkg item = new RecevePkg(remoteEP, array);
					lock (receveMessageQue)
					{
						receveMessageQue.Enqueue(item);
					}
					break;
				}
			}
			catch (Exception ex)
			{
				Util.LogWarning("reve message have a error :" + ex.ToString());
			}
			Thread.Sleep(100);
		}
	}

	public void Update()
	{
		if (receveMessageQue.Count != 0)
		{
			while (receveMessageQue.Count > 0)
			{
				RecevePkg pkg = receveMessageQue.Dequeue();
				HandleMessage(pkg);
			}
		}
	}

	private void HandleMessage(RecevePkg pkg)
	{
		if (pkg != null && pkg.data != null)
		{
			ByteBuffer byteBuffer = new ByteBuffer(pkg.data);
			int num = byteBuffer.ReadShort();
			Util.LogWarning("recve message message <<<<----- [id:" + num + "] [length:" + pkg.data.Length + "]");
			if (onReceveData != null)
			{
				onReceveData(num, byteBuffer);
			}
		}
	}

	public void Release()
	{
		udpClient.Close();
		readThread.Interrupt();
		receveMessageQue.Clear();
	}
}
