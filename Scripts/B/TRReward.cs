using System;
using UnityEngine;

[Serializable]
public class TRReward
{
	[SerializeField]
	private string transactionIdentifier;

	[SerializeField]
	private string currencyName;

	[SerializeField]
	private string placementIdentifier;

	[SerializeField]
	private int rewardAmount;

	[SerializeField]
	private int payoutEvent;

	public string TransactionIdentifier
	{
		get
		{
			return transactionIdentifier;
		}
	}

	public string CurrencyName
	{
		get
		{
			return currencyName;
		}
	}

	public string PlacementIdentifier
	{
		get
		{
			return placementIdentifier;
		}
	}

	public int RewardAmount
	{
		get
		{
			return rewardAmount;
		}
	}

	public int PayoutEvent
	{
		get
		{
			return payoutEvent;
		}
	}
}
