using TapjoyUnity.Internal;

namespace TapjoyUnity
{
	public sealed class TJActionRequest
	{
		public string requestID;

		public string token;

		internal TJActionRequest(string requestID, string token)
		{
			this.requestID = requestID;
			this.token = token;
		}

		~TJActionRequest()
		{
			if (requestID != null)
			{
				TapjoyComponent.RemoveActionRequest(requestID);
			}
		}

		public void Completed()
		{
			ApiBinding.Instance.ActionRequestCompleted(requestID);
		}

		public void Cancelled()
		{
			ApiBinding.Instance.ActionRequestCancelled(requestID);
		}
	}
}
