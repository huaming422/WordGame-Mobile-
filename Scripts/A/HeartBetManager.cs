using System;
using System.Threading;

public class HeartBetManager : SingleSysObj<HeartBetManager>
{
	private Thread heartBetThread;

	private DateTime lastTimeReciveTime;

	private int heartBetInterval = 3;

	private int serverInteval = 5;

	protected override void Init()
	{
		base.Init();
		lastTimeReciveTime = DateTime.Now;
	}

	public void Pinit()
	{
	}

	public void Start()
	{
		if (heartBetThread == null || heartBetThread.ThreadState != 0)
		{
			heartBetThread = new Thread(HeartBetThread);
			heartBetThread.Start();
		}
	}

	private void HeartBetThread()
	{
		while (!((DateTime.Now - lastTimeReciveTime).TotalSeconds > (double)serverInteval))
		{
			Thread.Sleep(heartBetInterval * 1000);
		}
		NetworkManager.Close();
	}

	public void StopHeartBet()
	{
		if (heartBetThread != null && heartBetThread.IsAlive)
		{
			try
			{
				heartBetThread.Abort();
			}
			catch (Exception ex)
			{
				LogManger.LogError("stop HeartBet is Error :---->>>>>" + ex.ToString());
			}
		}
		heartBetThread = null;
	}

	public void ResetLastReciveTime()
	{
		lastTimeReciveTime = DateTime.Now;
	}
}
