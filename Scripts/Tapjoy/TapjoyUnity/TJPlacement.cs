using System;
using System.Collections.Generic;
using TapjoyUnity.Internal;
using UnityEngine;

namespace TapjoyUnity
{
	public class TJPlacement
	{
		public delegate void OnRequestSuccessHandler(TJPlacement placement);

		public delegate void OnRequestFailureHandler(TJPlacement placement, string error);

		public delegate void OnContentReadyHandler(TJPlacement placement);

		public delegate void OnContentShowHandler(TJPlacement placement);

		public delegate void OnContentDismissHandler(TJPlacement placement);

		public delegate void OnPurchaseRequestHandler(TJPlacement placement, TJActionRequest request, string productId);

		public delegate void OnRewardRequestHandler(TJPlacement placement, TJActionRequest request, string itemId, int quantity);

		public delegate void OnVideoStartHandler(TJPlacement placement);

		public delegate void OnVideoErrorHandler(TJPlacement placement, string errorMessage);

		public delegate void OnVideoCompleteHandler(TJPlacement placement);

		private static Dictionary<string, WeakReference> placementDictionary = new Dictionary<string, WeakReference>();

		private string _placementName;

		private string _guid;

		internal static TapjoyAction<string> OnShowContentCalled;

		private static OnRequestSuccessHandler OnRequestSuccessInvoker;

		private static OnRequestFailureHandler OnRequestFailureInvoker;

		private static OnContentReadyHandler OnContentReadyInvoker;

		private static OnContentShowHandler OnContentShowInvoker;

		private static OnContentDismissHandler OnContentDismissInvoker;

		private static OnPurchaseRequestHandler OnPurchaseRequestInvoker;

		private static OnRewardRequestHandler OnRewardRequestInvoker;

		private static OnVideoStartHandler OnVideoStartInvoker;

		private static OnVideoErrorHandler OnVideoErrorInvoker;

		private static OnVideoCompleteHandler OnVideoCompleteInvoker;

		public static event OnRequestSuccessHandler OnRequestSuccess
		{
			add
			{
				OnRequestSuccessInvoker = (OnRequestSuccessHandler)Delegate.Combine(OnRequestSuccessInvoker, value);
			}
			remove
			{
				OnRequestSuccessInvoker = (OnRequestSuccessHandler)Delegate.Remove(OnRequestSuccessInvoker, value);
			}
		}

		public static event OnRequestFailureHandler OnRequestFailure
		{
			add
			{
				OnRequestFailureInvoker = (OnRequestFailureHandler)Delegate.Combine(OnRequestFailureInvoker, value);
			}
			remove
			{
				OnRequestFailureInvoker = (OnRequestFailureHandler)Delegate.Remove(OnRequestFailureInvoker, value);
			}
		}

		public static event OnContentReadyHandler OnContentReady
		{
			add
			{
				OnContentReadyInvoker = (OnContentReadyHandler)Delegate.Combine(OnContentReadyInvoker, value);
			}
			remove
			{
				OnContentReadyInvoker = (OnContentReadyHandler)Delegate.Remove(OnContentReadyInvoker, value);
			}
		}

		public static event OnContentShowHandler OnContentShow
		{
			add
			{
				OnContentShowInvoker = (OnContentShowHandler)Delegate.Combine(OnContentShowInvoker, value);
			}
			remove
			{
				OnContentShowInvoker = (OnContentShowHandler)Delegate.Remove(OnContentShowInvoker, value);
			}
		}

		public static event OnContentDismissHandler OnContentDismiss
		{
			add
			{
				OnContentDismissInvoker = (OnContentDismissHandler)Delegate.Combine(OnContentDismissInvoker, value);
			}
			remove
			{
				OnContentDismissInvoker = (OnContentDismissHandler)Delegate.Remove(OnContentDismissInvoker, value);
			}
		}

		public static event OnPurchaseRequestHandler OnPurchaseRequest
		{
			add
			{
				OnPurchaseRequestInvoker = (OnPurchaseRequestHandler)Delegate.Combine(OnPurchaseRequestInvoker, value);
			}
			remove
			{
				OnPurchaseRequestInvoker = (OnPurchaseRequestHandler)Delegate.Remove(OnPurchaseRequestInvoker, value);
			}
		}

		public static event OnRewardRequestHandler OnRewardRequest
		{
			add
			{
				OnRewardRequestInvoker = (OnRewardRequestHandler)Delegate.Combine(OnRewardRequestInvoker, value);
			}
			remove
			{
				OnRewardRequestInvoker = (OnRewardRequestHandler)Delegate.Remove(OnRewardRequestInvoker, value);
			}
		}

		public static event OnVideoStartHandler OnVideoStart
		{
			add
			{
				OnVideoStartInvoker = (OnVideoStartHandler)Delegate.Combine(OnVideoStartInvoker, value);
			}
			remove
			{
				OnVideoStartInvoker = (OnVideoStartHandler)Delegate.Remove(OnVideoStartInvoker, value);
			}
		}

		public static event OnVideoErrorHandler OnVideoError
		{
			add
			{
				OnVideoErrorInvoker = (OnVideoErrorHandler)Delegate.Combine(OnVideoErrorInvoker, value);
			}
			remove
			{
				OnVideoErrorInvoker = (OnVideoErrorHandler)Delegate.Remove(OnVideoErrorInvoker, value);
			}
		}

		public static event OnVideoCompleteHandler OnVideoComplete
		{
			add
			{
				OnVideoCompleteInvoker = (OnVideoCompleteHandler)Delegate.Combine(OnVideoCompleteInvoker, value);
			}
			remove
			{
				OnVideoCompleteInvoker = (OnVideoCompleteHandler)Delegate.Remove(OnVideoCompleteInvoker, value);
			}
		}

		private TJPlacement(string placementName)
		{
			string text = Guid.NewGuid().ToString();
			_placementName = placementName;
			_guid = text;
			WeakReference value = new WeakReference(this);
			placementDictionary.Add(text, value);
			ApiBinding.Instance.CreatePlacement(text, placementName);
		}

		~TJPlacement()
		{
			if (_guid != null)
			{
				TapjoyComponent.RemovePlacement(_guid);
			}
		}

		public static TJPlacement CreatePlacement(string placementName)
		{
			return new TJPlacement(placementName);
		}

		public static void DismissContent()
		{
			ApiBinding.Instance.DismissPlacementContent();
		}

		public void RequestContent()
		{
			if (Tapjoy.IsConnected)
			{
				ApiBinding.Instance.RequestPlacementContent(_guid);
			}
			else
			{
				Debug.Log("C#: Can not send placement becuause Tapjoy has not successfully connected.");
			}
		}

		public void ShowContent()
		{
			if (OnShowContentCalled != null)
			{
				OnShowContentCalled(_placementName);
			}
			ApiBinding.Instance.ShowPlacementContent(_guid);
		}

		public bool IsContentAvailable()
		{
			return ApiBinding.Instance.IsPlacementContentAvailable(_guid);
		}

		public bool IsContentReady()
		{
			return ApiBinding.Instance.IsPlacementContentReady(_guid);
		}

		public string GetName()
		{
			return _placementName;
		}

		internal static void DispatchPlacementEvent(string commaDelimitedMessage)
		{
			string[] array = commaDelimitedMessage.Split(',');
			string key = array[1];
			string placementName = Uri.UnescapeDataString(array[2]);
			WeakReference value;
			TJPlacement tJPlacement;
			if (placementDictionary.TryGetValue(key, out value))
			{
				tJPlacement = (TJPlacement)value.Target;
				if (tJPlacement == null)
				{
					placementDictionary.Remove(key);
					tJPlacement = new TJPlacement(placementName);
				}
			}
			else
			{
				tJPlacement = new TJPlacement(placementName);
			}
			switch (array[0])
			{
			case "OnPlacementRequestSuccess":
				if (OnRequestSuccessInvoker != null)
				{
					OnRequestSuccessInvoker(tJPlacement);
				}
				break;
			case "OnPlacementRequestFailure":
				if (array.Length == 4 && OnRequestFailureInvoker != null)
				{
					OnRequestFailureInvoker(tJPlacement, array[3]);
				}
				break;
			case "OnPlacementContentReady":
				if (OnContentReadyInvoker != null)
				{
					OnContentReadyInvoker(tJPlacement);
				}
				break;
			case "OnPlacementContentShow":
				if (OnContentShowInvoker != null)
				{
					OnContentShowInvoker(tJPlacement);
				}
				break;
			case "OnPlacementContentDismiss":
				if (OnContentDismissInvoker != null)
				{
					OnContentDismissInvoker(tJPlacement);
				}
				break;
			case "OnPurchaseRequest":
				if (array.Length == 6 && OnPurchaseRequestInvoker != null)
				{
					string requestID2 = array[3];
					string token2 = array[4];
					string productId = array[5];
					OnPurchaseRequestInvoker(tJPlacement, new TJActionRequest(requestID2, token2), productId);
				}
				break;
			case "OnRewardRequest":
				if (array.Length == 7 && OnRewardRequestInvoker != null)
				{
					string requestID = array[3];
					string token = array[4];
					string itemId = array[5];
					int quantity = int.Parse(array[6]);
					OnRewardRequestInvoker(tJPlacement, new TJActionRequest(requestID, token), itemId, quantity);
				}
				break;
			}
		}

		internal static void DispatchPlacementVideoEvent(string commaDelimitedMessage)
		{
			string[] array = commaDelimitedMessage.Split(',');
			string key = array[1];
			string placementName = Uri.UnescapeDataString(array[2]);
			WeakReference value;
			TJPlacement tJPlacement;
			if (placementDictionary.TryGetValue(key, out value))
			{
				tJPlacement = (TJPlacement)value.Target;
				if (tJPlacement == null)
				{
					placementDictionary.Remove(key);
					tJPlacement = new TJPlacement(placementName);
				}
			}
			else
			{
				tJPlacement = new TJPlacement(placementName);
			}
			switch (array[0])
			{
			case "OnVideoStart":
				if (OnVideoStartInvoker != null)
				{
					OnVideoStartInvoker(tJPlacement);
				}
				break;
			case "OnVideoError":
				if (array.Length == 4)
				{
					string errorMessage = array[3];
					if (OnVideoErrorInvoker != null)
					{
						OnVideoErrorInvoker(tJPlacement, errorMessage);
					}
				}
				break;
			case "OnVideoComplete":
				if (OnVideoCompleteInvoker != null)
				{
					OnVideoCompleteInvoker(tJPlacement);
				}
				break;
			}
		}
	}
}
