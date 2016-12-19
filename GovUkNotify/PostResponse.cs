namespace GovUkNotify
{
	public struct PostResponse
	{
		public PostResponseData Data;
	}

	public struct PostResponseData
	{
		public string Body;
		public PostResponseNotification Notification;
	}

	public struct PostResponseNotification
	{
		public string Id;
	}
}
