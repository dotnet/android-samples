using System;
using Android.App;
using Android.Util;
using Android.Content;
using Android.OS;

namespace ServicesDemo3
{
	public class UtcTimestamper
	{
		DateTime startTime;
		bool wasReset = false;

		public UtcTimestamper()
		{
			startTime = DateTime.UtcNow;
		}

		public string GetFormattedTimestamp()
		{
			TimeSpan duration = DateTime.UtcNow.Subtract(startTime);

			return wasReset ? $"Service restarted at {startTime} ({duration:c} ago)." : $"Service started at {startTime} ({duration:c} ago).";
		}

		public void Restart()
		{
			startTime = DateTime.UtcNow;
			wasReset = true;
		}
	}
}
