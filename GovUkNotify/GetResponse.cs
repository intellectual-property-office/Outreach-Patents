namespace GovUkNotify
{
	public struct GetResponse
	{
		public GetResponseData Data;
	}

	public struct GetResponseData
	{
		public GetResponseNotification Notification;
	}

	public struct GetResponseNotification
	{
		public string Status;
	}
}
