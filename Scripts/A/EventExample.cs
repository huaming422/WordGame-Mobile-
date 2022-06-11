using TapjoyUnity;
using UnityEngine;

public class EventExample : MonoBehaviour
{
	public string output = string.Empty;

	private const string DEFAULT_CATEGORY = "SDKTestCategory";

	private const string DEFAULT_EVENT_NAME = "SDKTestEvent";

	private const string DEFAULT_KEY = "TestKey1";

	private const int DEFAULT_VALUE = 100;

	private const string DEFAULT_KEY2 = "TestKey2";

	private const long DEFAULT_VALUE2 = 200L;

	private const string DEFAULT_KEY3 = "TestKey3";

	private const long DEFAULT_VALUE3 = 300L;

	private const string DEFAULT_PARAM1 = "Param1";

	private const string DEFAULT_PARAM2 = "Param2";

	private GUIStyle outputStyle;

	private int fontSize = 20;

	private float startY;

	private float centerX;

	private float buttonWidth;

	private float buttonHeight;

	private float buttonPadding;

	private float headerHeight;

	private float yPadding = 50f;

	private void Start()
	{
		Debug.Log("C#: EventExample Start");
		InitUI();
	}

	private void Update()
	{
	}

	private void InitUI()
	{
		outputStyle = new GUIStyle();
		outputStyle.alignment = TextAnchor.MiddleCenter;
		outputStyle.normal.textColor = Color.white;
		outputStyle.wordWrap = true;
		outputStyle.fontSize = fontSize;
		startY = Screen.height / 10;
		centerX = Screen.width / 2;
		buttonWidth = Screen.width / 3;
		buttonHeight = Screen.height / 15;
		buttonPadding = 5f;
		yPadding = buttonHeight + 10f;
	}

	private void OnGUI()
	{
		float num = startY;
		Rect position = new Rect(centerX - buttonWidth - buttonPadding, num, buttonWidth, buttonHeight);
		if (GUI.Button(position, "Basic"))
		{
			Tapjoy.TrackEvent("SDKTestEvent", 100L);
			output = "Sent track event with name: SDKTestEvent, " + 100;
		}
		position = new Rect(centerX + buttonPadding, num, buttonWidth, buttonHeight);
		if (GUI.Button(position, "Value"))
		{
			Tapjoy.TrackEvent("SDKTestCategory", "SDKTestEvent", 100L);
		}
		num += yPadding;
		position = new Rect(centerX - buttonWidth - buttonPadding, num, buttonWidth, buttonHeight);
		if (GUI.Button(position, "Param1"))
		{
			Tapjoy.TrackEvent("SDKTestCategory", "SDKTestEvent", "Param1", null, 0L);
		}
		position = new Rect(centerX + buttonPadding, num, buttonWidth, buttonHeight);
		if (GUI.Button(position, "Param 1 and 2"))
		{
			Tapjoy.TrackEvent("SDKTestCategory", "SDKTestEvent", "Param1", "Param2", 0L);
		}
		num += yPadding;
		position = new Rect(centerX - buttonWidth - buttonPadding, num, buttonWidth, buttonHeight);
		if (GUI.Button(position, "Param 1 with value 1"))
		{
			Tapjoy.TrackEvent("SDKTestCategory", "SDKTestEvent", "Param1", null, 100L);
		}
		position = new Rect(centerX + buttonPadding, num, buttonWidth, buttonHeight);
		if (GUI.Button(position, "Param 1 and 2 with value 1"))
		{
			Tapjoy.TrackEvent("SDKTestCategory", "SDKTestEvent", "Param1", "Param2", 100L);
		}
		num += yPadding;
		position = new Rect(centerX - buttonWidth - buttonPadding, num, buttonWidth, buttonHeight);
		if (GUI.Button(position, "Param 1 and 2 with value 1 and 2"))
		{
			Tapjoy.TrackEvent("SDKTestCategory", "SDKTestEvent", "Param1", "Param2", "TestKey1", 100L, "TestKey2", 200L, null, 0L);
		}
		position = new Rect(centerX + buttonPadding, num, buttonWidth, buttonHeight);
		if (GUI.Button(position, "All"))
		{
			Tapjoy.TrackEvent("SDKTestCategory", "SDKTestEvent", "Param1", "Param2", "TestKey1", 100L, "TestKey2", 200L, "TestKey3", 300L);
		}
		num += yPadding;
		GUI.Label(new Rect(centerX - 200f, num, 400f, 150f), output, outputStyle);
	}
}
