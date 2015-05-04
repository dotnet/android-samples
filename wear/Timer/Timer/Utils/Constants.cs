using System;
using Uri = Android.Net.Uri;

namespace Timer
{
	public class Constants
	{
		public const string START_TIME = "timer_start_time";
		public const string ORIGINAL_TIME = "timer_original_time";
		public const string DATA_ITEM_PATH = "/timer";
		public static readonly Uri URI_PATTERN_DATA_ITEMS =
			Uri.FromParts ("wear", DATA_ITEM_PATH, null);

		public const int NOTIFICATION_TIMER_COUNTDOWN = 1;
		public const int NOTIFICATION_TIMER_EXPIRED = 2;

		public const string ACTION_SHOW_ALARM = "com.android.example.clockwork.timer.ACTION_SHOW";
		public const string ACTION_DELETE_ALARM = "com.android.example.clockwork.timer.ACTION_DELETE";
		public const string ACTION_RESTART_ALARM = "com.android.example.clockwork.timer.ACTION_RESTART";
	}
}

