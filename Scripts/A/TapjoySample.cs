using TapjoyUnity;
using UnityEngine;

public class TapjoySample : MonoBehaviour
{
	private enum UIState
	{
		Placement,
		Event,
		User
	}

	private UIState uiState;

	private PlacementExample mainUIView;

	private EventExample eventUIView;

	private UserExample userUIView;

	public bool viewIsShowing;

	public bool isConnected;

	public bool isFirebaseInitialized;

	private GUIStyle labelStyle;

	private int fontSize = 24;

	private float centerX;

	private float tabWidth;

	private float tabHeight;

	private float yPadding = 50f;

	private void Start()
	{
		Debug.Log("C#: TapjoySample start and adding Tapjoy Delegates");
		mainUIView = base.gameObject.GetComponentsInChildren<PlacementExample>(true)[0];
		eventUIView = base.gameObject.GetComponentsInChildren<EventExample>(true)[0];
		userUIView = base.gameObject.GetComponentsInChildren<UserExample>(true)[0];
		Tapjoy.OnConnectSuccess += HandleConnectSuccess;
		Tapjoy.OnConnectFailure += HandleConnectFailure;
	}

	private void OnDisable()
	{
		Debug.Log("C#: Disabling and removing Tapjoy Delegates");
		Tapjoy.OnConnectSuccess -= HandleConnectSuccess;
		Tapjoy.OnConnectFailure -= HandleConnectFailure;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}

	public void HandleConnectSuccess()
	{
		Debug.Log("C#: Handle Connect Success");
		isConnected = true;
		ChangeState(UIState.Placement);
	}

	public void HandleConnectFailure()
	{
		Debug.Log("C#: Handle Connect Failure");
	}

	public void HandleViewWillOpen(int viewType)
	{
		Debug.Log("C#: HandleViewWillOpen, viewType: " + viewType);
	}

	public void HandleViewDidOpen(int viewType)
	{
		Debug.Log("C#: HandleViewDidOpen, viewType: " + viewType);
		viewIsShowing = true;
	}

	public void HandleViewWillClose(int viewType)
	{
		Debug.Log("C#: HandleViewWillClose, viewType: " + viewType);
	}

	public void HandleViewDidClose(int viewType)
	{
		Debug.Log("C#: HandleViewDidClose, viewType: " + viewType);
		viewIsShowing = false;
	}

	private void OnGUI()
	{
		if (!viewIsShowing)
		{
			labelStyle = new GUIStyle();
			labelStyle.alignment = TextAnchor.MiddleCenter;
			labelStyle.normal.textColor = Color.white;
			labelStyle.wordWrap = true;
			labelStyle.fontSize = fontSize;
			centerX = Screen.width / 2;
			tabWidth = Screen.width / 3;
			tabHeight = Screen.height / 20;
			yPadding = tabHeight + 10f;
			float num = 0f;
			if (uiState == UIState.Placement || !isConnected)
			{
				GUI.enabled = false;
			}
			Rect position = new Rect(centerX - (tabWidth + tabWidth / 2f), num, tabWidth, tabHeight);
			if (GUI.Button(position, "Placement"))
			{
				ChangeState(UIState.Placement);
			}
			if (uiState == UIState.Placement || !isConnected)
			{
				GUI.enabled = true;
			}
			if (uiState == UIState.Event || !isConnected)
			{
				GUI.enabled = false;
			}
			position = new Rect(centerX - tabWidth / 2f, num, tabWidth, tabHeight);
			if (GUI.Button(position, "Event"))
			{
				ChangeState(UIState.Event);
			}
			if (uiState == UIState.Event || !isConnected)
			{
				GUI.enabled = true;
			}
			if (uiState == UIState.User || !isConnected)
			{
				GUI.enabled = false;
			}
			position = new Rect(centerX + tabWidth / 2f, num, tabWidth, tabHeight);
			if (GUI.Button(position, "User"))
			{
				ChangeState(UIState.User);
			}
			if (uiState == UIState.User || !isConnected)
			{
				GUI.enabled = true;
			}
			num += yPadding;
			position = new Rect(centerX - 200f, num, 400f, 25f);
			GUI.Label(position, "Tapjoy Connect Sample App", labelStyle);
			if (!isConnected)
			{
				num += yPadding;
				position = new Rect(centerX - 200f, num, 400f, 25f);
				GUI.Label(position, "Trying to connect to Tapjoy...", labelStyle);
			}
		}
	}

	private void ChangeState(UIState state)
	{
		Debug.Log("C#: change state: " + state);
		switch (state)
		{
		case UIState.Placement:
			mainUIView.enabled = true;
			eventUIView.enabled = false;
			userUIView.enabled = false;
			break;
		case UIState.Event:
			mainUIView.enabled = false;
			eventUIView.enabled = true;
			userUIView.enabled = false;
			break;
		case UIState.User:
			mainUIView.enabled = false;
			eventUIView.enabled = false;
			userUIView.enabled = true;
			break;
		}
		uiState = state;
	}
}
