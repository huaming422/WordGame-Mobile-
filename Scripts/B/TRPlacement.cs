using System;
using UnityEngine;

[Serializable]
public class TRPlacement
{
	[SerializeField]
	private string placementIdentifier;

	[SerializeField]
	private string currencyName;

	[SerializeField]
	private string errorMessage;

	[SerializeField]
	private bool isSurveyWallAvailable;

	[SerializeField]
	private bool hasHotSurvey;

	[SerializeField]
	private int placementCode;

	[SerializeField]
	private int maxPayoutInCurrency;

	[SerializeField]
	private int minPayoutInCurrency;

	[SerializeField]
	private int maxSurveyLength;

	[SerializeField]
	private int minSurveyLength;

	public const int PLACEMENT_CODE_SDK_NOT_READY = -1;

	public string PlacementIdentifier
	{
		get
		{
			return placementIdentifier;
		}
	}

	public string CurrencyName
	{
		get
		{
			return currencyName;
		}
	}

	public string ErrorMessage
	{
		get
		{
			return errorMessage;
		}
	}

	public bool IsSurveyWallAvailable
	{
		get
		{
			return isSurveyWallAvailable;
		}
	}

	public bool HasHotSurvey
	{
		get
		{
			return hasHotSurvey;
		}
	}

	public int PlacementCode
	{
		get
		{
			return placementCode;
		}
	}

	public int MaxPayoutInCurrency
	{
		get
		{
			return maxPayoutInCurrency;
		}
	}

	public int MinPayoutInCurrency
	{
		get
		{
			return minPayoutInCurrency;
		}
	}

	public int MaxSurveyLength
	{
		get
		{
			return maxSurveyLength;
		}
	}

	public int MinSurveyLength
	{
		get
		{
			return minSurveyLength;
		}
	}

	public void ShowSurveyWall()
	{
		isSurveyWallAvailable = false;
		TapResearch.ShowSurveyWall(PlacementIdentifier);
	}
}
