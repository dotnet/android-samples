using System;
using Android.App;
using Android.Gms.Analytics;
using Android;
using Android.Runtime;

namespace Analytics
{
	[Application (Label = "@string/app_name", Icon = "@drawable/icon", Theme = "@style/AppTheme", AllowBackup = true)]
	public class AnalyticsApplication : Application
	{
		Tracker mTracker;
		public Tracker DefaultTracker {
			get {
				if (mTracker == null) {
					var analytics = GoogleAnalytics.GetInstance (this);
					// Add your Tracking ID here
					mTracker = analytics.NewTracker ("UA-XXXXXXXX-X");
				}
				return mTracker;
			}
		}

		public AnalyticsApplication (IntPtr handle, JniHandleOwnership ownerShip) 
			: base (handle, ownerShip)
		{
		}
	}
}

