using System;
namespace ServicesDemo3
{
	public static class Constants
	{
		public const int DELAY_BETWEEN_LOG_MESSAGES = 5000; // milliseconds
		public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;
		public const string SERVICE_STARTED_KEY = "has_service_been_started";
		public const string BROADCAST_MESSAGE_KEY = "broadcast_message";
		public const string NOTIFICATION_BROADCAST_ACTION = "ServicesDemo3.Notification.Action";

		public const string ACTION_START_SERVICE = "ServicesDemo3.action.START_SERVICE";
		public const string ACTION_STOP_SERVICE = "ServicesDemo3.action.STOP_SERVICE";
		public const string ACTION_RESTART_TIMER = "ServicesDemo3.action.RESTART_TIMER";
		public const string ACTION_MAIN_ACTIVITY = "ServicesDemo3.action.MAIN_ACTIVITY";
	}
}
