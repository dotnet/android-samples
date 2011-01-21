using System;

using Android.App;
using Android.Content;
using Android.Widget;

namespace MonoDroid.ApiDemo {

	[BroadcastReceiver]
	public class RepeatingAlarm : BroadcastReceiver
	{
		public override void OnReceive (Context context, Intent intent)
		{
			Toast.MakeText (context, Resource.String.repeating_received, ToastLength.Short).Show ();
		}
	}
}

