
namespace GovUkNotify
{
	public interface INotifications
	{
		PostResponse SendSms(string data);

		GetResponse GetNotification(string id);
	}
}
