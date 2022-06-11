using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LogManger : SingleObject<LogManger>
{
	private class LogItem
	{
		public LogType type = LogType.Log;

		public string message = string.Empty;

		public DateTime time;

		public LogItem(LogType type, string message, DateTime time)
		{
			this.time = time;
			this.type = type;
			this.message = message;
		}
	}

	private static Queue<LogItem> logs = new Queue<LogItem>();

	private int perframeMaxLogsCount = 20;

	private int nowLogCount;

	private LogType loglevel = LogType.Log;

	private bool listenApplictionLog;

	private bool logToFile = true;

	private string logFilePath
	{
		get
		{
			return AppConst.TempDir + "app.log";
		}
	}

	public void Init(bool isListenApplicaton = false, bool isLogToFile = true)
	{
		listenApplictionLog = isListenApplicaton;
		logToFile = isLogToFile;
		if (isListenApplicaton)
		{
			Application.logMessageReceived += ApplicationLog;
		}
		if (!isLogToFile)
		{
			return;
		}
		string directoryName = Path.GetDirectoryName(logFilePath);
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		if (File.Exists(logFilePath))
		{
			FileInfo fileInfo = new FileInfo(logFilePath);
			if (fileInfo.Length > 5242880)
			{
				File.WriteAllText(logFilePath, string.Empty);
			}
		}
	}

	private void ApplicationLog(string condition, string stackTrace, LogType type)
	{
		if (logToFile)
		{
			string arg = ((type != 0 && type != LogType.Exception) ? condition : (condition + " error info:------>>>>>" + stackTrace));
			string arg2 = DateTime.Now.ToString("yy-MM-dd HH:mm:ss");
			string contents = string.Format("{0}:{1}-->>>{2}\r\n", arg2, type.ToString(), arg);
			File.AppendAllText(logFilePath, contents);
		}
	}

	private void Update()
	{
		if (logs.Count != 0)
		{
			FetchLog();
		}
	}

	private void FetchLog()
	{
		nowLogCount = 0;
		while (logs.Count > 0 && nowLogCount < perframeMaxLogsCount)
		{
			lock (logs)
			{
				LogItem item = logs.Dequeue();
				Log(item);
			}
			nowLogCount++;
		}
	}

	private static void AddLogItem(LogItem item)
	{
		if (item != null)
		{
			lock (logs)
			{
				logs.Enqueue(item);
			}
		}
	}

	private void Log(LogItem item)
	{
		if (item != null)
		{
			switch (item.type)
			{
			case LogType.Log:
				Util.Log(item.message);
				break;
			case LogType.Warning:
				Util.LogWarning(item.message);
				break;
			case LogType.Error:
				Util.LogError(item.message);
				break;
			}
		}
	}

	public static void Log(string log)
	{
		LogItem item = new LogItem(LogType.Log, log, DateTime.Now);
		AddLogItem(item);
	}

	public static void LogWaring(string log)
	{
		LogItem item = new LogItem(LogType.Warning, log, DateTime.Now);
		AddLogItem(item);
	}

	public static void LogError(string log)
	{
		LogItem item = new LogItem(LogType.Error, log, DateTime.Now);
		AddLogItem(item);
	}
}
