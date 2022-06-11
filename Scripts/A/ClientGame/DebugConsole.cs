using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClientGame
{
	public class DebugConsole : MonoBehaviour
	{
		private static readonly int MAX_LOG = 1000;

		private static readonly int WND_ID = 5173;

		private static readonly float EDGE_X = 12f;

		private static readonly float EDGE_Y = 12f;

		public bool Visible;

		private readonly string[] logTypeNames_;

		private readonly Queue<string>[] logList_;

		private readonly Vector2[] scrollPos_;

		private int fre;

		private int curFre;

		private float time;

		private float CoolDown_;

		private int logTypeChoose_ = 3;

		private Rect rcWindow_;

		public static DebugConsole Instance { get; private set; }

		private DebugConsole()
		{
			logTypeNames_ = Enum.GetNames(typeof(LogType));
			logList_ = new Queue<string>[logTypeNames_.Length];
			scrollPos_ = new Vector2[logTypeNames_.Length];
			for (int i = 0; i < logList_.Length; i++)
			{
				logList_[i] = new Queue<string>(MAX_LOG);
				scrollPos_[i] = new Vector2(0f, 1f);
			}
		}

		public static void Create()
		{
			GameObject gameObject = new GameObject("DebugConsole");
			Instance = gameObject.AddComponent<DebugConsole>();
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
		}

		protected void Awake()
		{
			Application.logMessageReceivedThreaded += LogCallback;
		}

		protected void OnDisable()
		{
			Application.logMessageReceivedThreaded -= LogCallback;
		}

		protected void Start()
		{
			if (Application.isMobilePlatform)
			{
				Visible = false;
			}
		}

		protected void Update()
		{
			time += Time.deltaTime;
			fre++;
			if (time > 1f)
			{
				time = 0f;
				curFre = fre;
				fre = 0;
			}
			if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
			{
				if (Input.GetMouseButton(0) && Input.GetMouseButton(1) && Time.time - CoolDown_ > 2f)
				{
					Visible = !Visible;
					CoolDown_ = Time.time;
				}
			}
			else if (Input.touches.Length >= 3 && Time.time - CoolDown_ > 2f)
			{
				Visible = !Visible;
				CoolDown_ = Time.time;
			}
		}

		protected void OnGUI()
		{
			if (GUI.Button(new Rect(Screen.width - 250, 2f, 100f, 40f), "open"))
			{
				Visible = !Visible;
			}
			if (!Visible)
			{
				return;
			}
			EventType type = Event.current.type;
			if (type == EventType.Repaint || type == EventType.Layout)
			{
				rcWindow_ = new Rect(EDGE_X, EDGE_Y * 2f, (float)Screen.width - EDGE_X * 2f, (float)Screen.height - EDGE_Y * 2f);
				GUI.Window(WND_ID, rcWindow_, WindowFunc, GUIContent.none);
			}
			if (GUI.Button(new Rect(Screen.width - 100, 2f, 100f, 40f), "Clear"))
			{
				Queue<string>[] array = logList_;
				foreach (Queue<string> queue in array)
				{
					queue.Clear();
				}
			}
		}

		private void WindowFunc(int id)
		{
			try
			{
				GUILayout.BeginVertical();
				try
				{
					logTypeChoose_ = GUILayout.Toolbar(logTypeChoose_, logTypeNames_);
					Queue<string> queue = logList_[logTypeChoose_];
					if (queue.Count <= 0)
					{
						return;
					}
					scrollPos_[logTypeChoose_] = GUILayout.BeginScrollView(scrollPos_[logTypeChoose_]);
					try
					{
						foreach (string item in queue)
						{
							GUILayout.Label(item);
						}
					}
					finally
					{
						GUILayout.EndScrollView();
					}
				}
				finally
				{
					GUILayout.EndVertical();
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		private static void Enqueue(Queue<string> queue, string text, string stackTrace)
		{
			while (queue.Count >= MAX_LOG)
			{
				queue.Dequeue();
			}
			queue.Enqueue(text);
		}

		private void LogCallback(string condition, string stackTrace, LogType type)
		{
			Queue<string> queue = logList_[(int)type];
			switch (type)
			{
			case LogType.Error:
			case LogType.Warning:
			case LogType.Exception:
				Enqueue(queue, condition, stackTrace);
				break;
			default:
				Enqueue(queue, condition, null);
				break;
			}
			scrollPos_[(int)type] = new Vector2(0f, 100000f);
		}
	}
}
