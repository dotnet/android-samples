using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.GoogleAnalyticsV2.Tracking;

namespace AnalyticsV2Test
{
	[Activity (Label = "AnalyticsV2Test", MainLauncher = true)]
	public class TestActivity : Activity
	{
		Tracker tracker;
		
		protected override void OnStart ()
		{
			base.OnStart ();
			// set your tracking code here.
			tracker = GoogleAnalytics.GetInstance (this).GetTracker ("UA-XXXX-Y");

			tracker.SendView ("/HomeScreen");
		}

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.main);
			Button createEventButton = (Button)FindViewById (Resource.Id.NewEventButton);
			createEventButton.Click += delegate {
				tracker.TrackEvent (
					"Clicks",  // Category
					"Button",  // Action
					"clicked", // Label
					new Java.Lang.Long (77));       // Value
			};

			Button createPageButton = (Button)FindViewById (Resource.Id.NewPageButton);
			createPageButton.Click += delegate {
				// Add a Custom Variable to this pageview, with name of "Medium" and value "MobileApp"
				tracker.Set("Medium", "Mobile App");
				// Track a page view. This is probably the best way to track which parts of your application
				// are being used.
				// E.g.
				// tracker.trackPageView("/help"); to track someone looking at the help screen.
				// tracker.trackPageView("/level2"); to track someone reaching level 2 in a game.
				// tracker.trackPageView("/uploadScreen"); to track someone using an upload screen.
				tracker.TrackView ("/testApplicationHomeScreen");
			};
			Button quitButton = (Button)FindViewById (Resource.Id.QuitButton);
			quitButton.Click += delegate {
				Finish ();
			};
			Button dispatchButton = (Button)FindViewById (Resource.Id.DispatchButton);
			dispatchButton.Click += delegate {
				// Manually start a dispatch, not needed if the tracker was started with a dispatch
				// interval.
				GAServiceManager.Instance.Dispatch ();
			};
		}
	}
}

