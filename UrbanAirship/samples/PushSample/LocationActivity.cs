using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.UrbanAirship;
using Xamarin.UrbanAirship.Locations;
using Xamarin.UrbanAirship.Utils;

namespace PushSample
{
	[Activity (Label = "LocationActivity")]			
	public class LocationActivity : Activity
	{
		public LocationActivity ()
		{
			locationUpdateReceiver = new DelegateBroadcastReceiver (LocationUpdateReceiver_OnReceive);
		}

		Button networkUpdateButton;
		Button gpsUpdateButton;
		Criteria newCriteria;
		Drawable mapIcon;
		IntentFilter locationFilter;
		LinearLayout mapLayout;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.location);

			networkUpdateButton = (Button)FindViewById (Resource.Id.network_update_button);
			gpsUpdateButton = (Button)FindViewById (Resource.Id.gps_update_button);

			locationFilter = new IntentFilter ();
			locationFilter.AddAction (UALocationManager.ActionLocationUpdate);

			newCriteria = new Criteria ();
			newCriteria.Accuracy = Accuracy.Fine;

			networkUpdateButton.Click += delegate {
				try {
					UALocationManager.Shared ().RecordCurrentLocation ();
				} catch (ServiceNotBoundException e) {
					Logger.Debug (e.Message);
				} catch (RemoteException e) {
					Logger.Debug (e.Message);
				}
			};

			gpsUpdateButton.Click += delegate {

				try {
					UALocationManager.Shared ().RecordCurrentLocation (newCriteria);
				} catch (ServiceNotBoundException e) {
					Logger.Debug (e.Message);
				} catch (RemoteException e) {
					Logger.Debug (e.Message);
				}
			};


		}
		// Implementing onStart/onStop because we're not extending a UA activity
		protected override void OnStart ()
		{
			base.OnStart ();
			UAirship.Shared ().Analytics.ActivityStarted (this);
		}

		protected override void OnStop ()
		{
			base.OnStop ();
			UAirship.Shared ().Analytics.ActivityStopped (this);
		}

		protected override void OnResume ()
		{
			base.OnResume ();
			RegisterReceiver (locationUpdateReceiver, locationFilter);
		}

		protected override void OnPause ()
		{
			base.OnPause ();
			UnregisterReceiver (locationUpdateReceiver);
		}

		class DelegateBroadcastReceiver : BroadcastReceiver
		{
			public DelegateBroadcastReceiver (Action<Context,Intent> onReceive)
			{
				this.onReceive = onReceive;
			}

			Action<Context,Intent> onReceive;

			public override void OnReceive (Context context, Intent intent)
			{
				onReceive (context, intent);
			}
		}

		BroadcastReceiver locationUpdateReceiver;

		void LocationUpdateReceiver_OnReceive (Context context, Intent intent)
		{
			if (UALocationManager.ActionLocationUpdate == intent.Action) {
				Location newLocation = (Location)intent.Extras.Get (UALocationManager.LocationKey);

				String text = string.Format ("lat: {0}, lon: {1}", newLocation.Latitude, newLocation.Longitude);

				Toast.MakeText (UAirship.Shared ().ApplicationContext,
					text, ToastLength.Long).Show ();
			}
		}
	}
}