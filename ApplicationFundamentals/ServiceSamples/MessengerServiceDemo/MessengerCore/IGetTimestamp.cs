using System;
using Android.App;

namespace MessengerCore
{
	/// <summary>
	/// The interface used to get a formatted timestamp string from the Timestamp Service.
	/// </summary>
	public interface IGetTimestamp
	{
		string GetFormattedTimestamp();
	}
	
}
