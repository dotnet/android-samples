using System;
using Android.App;
using Android.Util;
using Android.Content;
using Android.OS;

namespace StartedServicesDemo
{

	public class UtcTimestamper
	{
		DateTime startTime;

		public UtcTimestamper()
		{
			startTime = DateTime.UtcNow;
		}

		public string GetFormattedTimestamp()
		{
			TimeSpan duration = DateTime.UtcNow.Subtract(startTime);
			return $"Service started at {startTime} ({duration:c} ago).";
		}

	}
	
}
