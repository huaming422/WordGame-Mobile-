using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class OutLog : MonoBehaviour
{
	private static List<string> mLines = new List<string>();

	private static List<string> mWriteTxt = new List<string>();

	private string outpath;

	private void Start()
	{
		outpath = Application.persistentDataPath + "/outLog.txt";
		if (File.Exists(outpath))
		{
			File.Delete(outpath);
		}
		Application.logMessageReceived += HandleLog;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Update()
	{
		if (mWriteTxt.Count <= 0)
		{
			return;
		}
		string[] array = mWriteTxt.ToArray();
		string[] array2 = array;
		foreach (string text in array2)
		{
			using (StreamWriter streamWriter = new StreamWriter(outpath, true, Encoding.UTF8))
			{
				streamWriter.WriteLine(text);
			}
			mWriteTxt.Remove(text);
		}
	}

	private void HandleLog(string logString, string stackTrace, LogType type)
	{
		mWriteTxt.Add(logString);
		if (type == LogType.Error || type == LogType.Exception)
		{
			Log(logString);
			Log(stackTrace);
		}
	}

	public static void Log(params object[] objs)
	{
		string text = string.Empty;
		for (int i = 0; i < objs.Length; i++)
		{
			text = ((i != 0) ? (text + ", " + objs[i].ToString()) : (text + objs[i].ToString()));
		}
		if (Application.isPlaying)
		{
			if (mLines.Count > 20)
			{
				mLines.RemoveAt(0);
			}
			mLines.Add(text);
		}
	}

	private void OnGUI()
	{
		GUI.color = Color.red;
		int i = 0;
		for (int count = mLines.Count; i < count; i++)
		{
			GUILayout.Label(mLines[i]);
		}
	}
}
