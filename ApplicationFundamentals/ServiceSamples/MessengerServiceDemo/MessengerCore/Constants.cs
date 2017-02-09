using System;
namespace MessengerCore
{
	/// <summary>
	/// Constants that are shared between the Timestamp Service and it's clients.
	/// </summary>
	public static class Constants
	{
		public const string TIMESTAMP_SERVICE_PERMISSION = "com.xamarin.xample.messengerservice.REQUEST_TIMESTAMP";
		public const int TIMESTAMP_SERVICE_REQUEST_PERMISSION = 8000;

		public const string REMOTE_SERVICE_COMPONENT_NAME = "com.xamarin.TimestampService";
		public const string REMOTE_SERVICE_PACKAGE_NAME = "com.xamarin.xample.messengerservice";

		public const int SAY_HELLO_TO_TIMESTAMP_SERVICE = 9000;

		public const int GET_UTC_TIMESTAMP = 10000;
		public const int START_TIMESTAMP_SERVICE = 10010;
		public const int STOP_TIMESTAMP_SERVICE = 10020;
		public const int GET_UTC_TIMESTAMP_RESPONSE = 10100;

		public const string TIMESTAMP_RESPONSE_KEY = "timestamp_message";
	}
}
