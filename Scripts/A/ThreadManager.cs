using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;

public class ThreadManager : SingleObject<ThreadManager>
{
	private delegate void ThreadSyncEvent(NotiData data);

	private Thread thread;

	private Action<NotiData> func;

	private Stopwatch sw = new Stopwatch();

	private string currDownFile = string.Empty;

	private static readonly object m_lockObject = new object();

	private static Queue<ThreadEvent> events = new Queue<ThreadEvent>();

	private ThreadSyncEvent m_SyncEvent;

	private void Awake()
	{
		m_SyncEvent = OnSyncEvent;
		thread = new Thread(OnUpdate);
	}

	private void Start()
	{
		thread.Start();
	}

	public void AddEvent(ThreadEvent ev, Action<NotiData> func)
	{
		lock (m_lockObject)
		{
			this.func = func;
			events.Enqueue(ev);
		}
	}

	private void OnSyncEvent(NotiData data)
	{
		if (func != null)
		{
			func(data);
		}
	}

	private void OnUpdate()
	{
		while (true)
		{
			lock (m_lockObject)
			{
				if (events.Count > 0)
				{
					ThreadEvent threadEvent = events.Dequeue();
					try
					{
						switch (threadEvent.Key)
						{
						case "UpdateExtract":
							OnExtractFile(threadEvent.evParams);
							break;
						case "UpdateDownload":
							OnDownloadFile(threadEvent.evParams);
							break;
						}
					}
					catch (Exception ex)
					{
						Util.LogError(ex.Message);
					}
				}
			}
			Thread.Sleep(1);
		}
	}

	private void OnDownloadFile(List<object> evParams)
	{
		string uriString = evParams[0].ToString();
		currDownFile = evParams[1].ToString();
		using (WebClient webClient = new WebClient())
		{
			sw.Start();
			webClient.DownloadProgressChanged += ProgressChanged;
			webClient.DownloadFileAsync(new Uri(uriString), currDownFile);
		}
	}

	private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
	{
		string param = string.Format("{0} kb/s", ((double)e.BytesReceived / 1024.0 / sw.Elapsed.TotalSeconds).ToString("0.00"));
		NotiData data = new NotiData("UpdateProgress", param);
		if (m_SyncEvent != null)
		{
			m_SyncEvent(data);
		}
		if (e.ProgressPercentage == 100 && e.BytesReceived == e.TotalBytesToReceive)
		{
			sw.Reset();
			data = new NotiData("UpdateDownload", currDownFile);
			if (m_SyncEvent != null)
			{
				m_SyncEvent(data);
			}
		}
	}

	private void OnExtractFile(List<object> evParams)
	{
		Util.LogWarning("Thread evParams: >>" + evParams.Count);
		NotiData data = new NotiData("UpdateDownload", null);
		if (m_SyncEvent != null)
		{
			m_SyncEvent(data);
		}
	}

	private void OnDestroy()
	{
		thread.Abort();
	}
}
