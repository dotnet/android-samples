using System;
using MessengerCore;

namespace MessengerService
{
	/// <summary>
	/// A simple implementation of the Timestamp service. The timestamp returns the date & time the service started.
	/// </summary>
	public class UtcTimestamper : IGetTimestamp
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
