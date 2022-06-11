using TapjoyUnity;
using UnityEngine;

public class UserExample : MonoBehaviour
{
	private const string inputUserIdPlaceholder = "User Id";

	private const string inputUserLevelPlaceholder = "Level";

	private const string inputUserFriendsPlaceholder = "Friend Count";

	private const string inputCohortPlaceholder = "Cohort";

	private const string inputUserTagPlaceholder = "Tag";

	private string inputUserId = "User Id";

	private string inputUserLevel = "Level";

	private string inputUserFriends = "Friend Count";

	private string[] cohortValues = new string[5];

	private string userTag = "Tag";

	private string currentUserTags = string.Empty;

	private GUIStyle inputStyle;

	private GUIStyle labelStyle;

	private GUIStyle outputStyle;

	private GUIStyle headerStyle;

	private int fontSize = 20;

	private float startX;

	private float startY;

	private float centerX;

	private float inputWidth;

	private float labelWidth;

	private float buttonWidth;

	private float buttonHeight;

	private float buttonPadding;

	private float yPadding = 50f;

	private void OnEnable()
	{
		Debug.Log("C# UserExample Enable -- Adding Tapjoy User ID delegates");
		Tapjoy.OnSetUserIDSuccess += HandleSetUserIDSuccess;
		Tapjoy.OnSetUserIDFailure += HandleSetUserIDFailure;
	}

	private void OnDisable()
	{
		Debug.Log("C#: UserExample -- Disabling and removing Tapjoy User ID Delegates");
		Tapjoy.OnSetUserIDSuccess -= HandleSetUserIDSuccess;
		Tapjoy.OnSetUserIDFailure -= HandleSetUserIDFailure;
	}

	private void HandleSetUserIDSuccess()
	{
		Debug.Log("C#: HandleSetUserIDSuccess");
	}

	private void HandleSetUserIDFailure(string error)
	{
		Debug.Log("C#: HandleSetUserIDFailure: " + error);
	}

	private void Start()
	{
		Debug.Log("C#: UserExample Start");
		InitUI();
	}

	private void Update()
	{
	}

	private void InitUI()
	{
		headerStyle = new GUIStyle();
		headerStyle.alignment = TextAnchor.MiddleLeft;
		headerStyle.normal.textColor = Color.white;
		headerStyle.fontSize = fontSize;
		labelStyle = new GUIStyle();
		labelStyle.alignment = TextAnchor.MiddleLeft;
		labelStyle.normal.textColor = Color.white;
		labelStyle.fontSize = fontSize;
		outputStyle = new GUIStyle();
		outputStyle.alignment = TextAnchor.MiddleCenter;
		outputStyle.normal.textColor = Color.white;
		outputStyle.wordWrap = true;
		outputStyle.fontSize = fontSize;
		centerX = Screen.width / 2;
		startX = centerX - (float)Screen.width * 0.4f;
		startY = Screen.height / 10;
		buttonPadding = 5f;
		inputWidth = (float)Screen.width * 0.6f;
		buttonWidth = (float)Screen.width * 0.8f / 2f;
		labelWidth = (float)Screen.width * 0.2f;
		buttonHeight = Screen.height / 25;
		yPadding = buttonHeight + 10f;
		getUserTags();
	}

	private void getUserTags()
	{
		currentUserTags = string.Empty;
		foreach (string userTag in Tapjoy.GetUserTags())
		{
			currentUserTags = currentUserTags + userTag + ", ";
		}
	}

	private void OnGUI()
	{
		float num = startY;
		if (inputStyle == null)
		{
			inputStyle = GUI.skin.textField;
			inputStyle.fontSize = fontSize;
		}
		Rect position = new Rect(startX, num, labelWidth, buttonHeight);
		GUI.Label(position, "User Id:", labelStyle);
		position = new Rect(startX + labelWidth, num, inputWidth, buttonHeight);
		inputUserId = GUI.TextField(position, inputUserId, 30, inputStyle);
		num += yPadding;
		position = new Rect(startX, num, inputWidth, buttonHeight);
		GUI.Label(position, "User Level:", labelStyle);
		position = new Rect(startX + labelWidth, num, inputWidth, buttonHeight);
		inputUserLevel = GUI.TextField(position, string.Empty + inputUserLevel, 30, inputStyle);
		num += yPadding;
		position = new Rect(startX, num, inputWidth, buttonHeight);
		GUI.Label(position, "User Friends:", labelStyle);
		position = new Rect(startX + labelWidth, num, inputWidth, buttonHeight);
		inputUserFriends = GUI.TextField(position, string.Empty + inputUserFriends, 30, inputStyle);
		num += yPadding;
		position = new Rect(startX, num, inputWidth, buttonHeight);
		GUI.Label(position, "User Cohorts:", headerStyle);
		num += yPadding;
		for (int i = 0; i < 5; i++)
		{
			position = new Rect(startX, num, inputWidth, buttonHeight);
			GUI.Label(position, "  Cohort " + (i + 1) + ":", labelStyle);
			position = new Rect(startX + labelWidth, num, inputWidth, buttonHeight);
			cohortValues[i] = GUI.TextField(position, string.Empty + cohortValues[i], 30, inputStyle);
			num += yPadding;
		}
		num += yPadding;
		position = new Rect(centerX - buttonWidth - buttonPadding, num, buttonWidth, buttonHeight);
		if (GUI.Button(position, "Set"))
		{
			Tapjoy.SetUserID(inputUserId);
			int result = 0;
			if (int.TryParse(inputUserLevel, out result))
			{
				Tapjoy.SetUserLevel(result);
			}
			if (int.TryParse(inputUserFriends, out result))
			{
				Tapjoy.SetUserFriendCount(result);
			}
			for (int j = 0; j < cohortValues.Length; j++)
			{
				Tapjoy.SetUserCohortVariable(j + 1, cohortValues[j]);
			}
		}
		position = new Rect(centerX + buttonPadding, num, buttonWidth, buttonHeight);
		if (GUI.Button(position, "Clear"))
		{
			inputUserId = "User Id";
			inputUserLevel = "Level";
			inputUserFriends = "Friend Count";
			cohortValues = new string[5];
		}
		num += yPadding;
		position = new Rect(startX, num, inputWidth, buttonHeight);
		GUI.Label(position, "User Tag:", labelStyle);
		position = new Rect(startX + labelWidth, num, inputWidth, buttonHeight);
		userTag = GUI.TextField(position, string.Empty + userTag, 30, inputStyle);
		num += yPadding;
		position = new Rect(centerX - buttonWidth - buttonPadding, num, buttonWidth * 2f / 3f, buttonHeight);
		if (GUI.Button(position, "Add"))
		{
			Tapjoy.AddUserTag(userTag);
			userTag = string.Empty;
			getUserTags();
		}
		position = new Rect(centerX - buttonWidth / 3f, num, buttonWidth * 2f / 3f, buttonHeight);
		if (GUI.Button(position, "Remove"))
		{
			Tapjoy.RemoveUserTag(userTag);
			userTag = string.Empty;
			getUserTags();
		}
		position = new Rect(centerX + buttonWidth / 3f + buttonPadding, num, buttonWidth * 2f / 3f, buttonHeight);
		if (GUI.Button(position, "Clear"))
		{
			Tapjoy.ClearUserTags();
			userTag = string.Empty;
			getUserTags();
		}
		num += yPadding;
		position = new Rect(centerX - buttonWidth / 2f - buttonPadding, num, buttonWidth, buttonHeight);
		if (GUI.Button(position, "Get User Tags"))
		{
			getUserTags();
		}
		num += yPadding;
		position = new Rect(centerX - buttonWidth - buttonPadding, num, buttonWidth, buttonHeight);
		GUI.Label(position, "User Tags: " + currentUserTags);
		num += yPadding;
	}
}
