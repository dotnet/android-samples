using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V4.App;
using Android.Content;
using Android.Gms.Location;
using Android.Preferences;
using Android;
using Android.Content.PM;
using Android.Util;
using Android.Support.Design.Widget;
using Android.Net;
using Android.Support.Compat;
using Android.Views;
using Java.Lang;
using Android.Support.V4.Content;

namespace LocUpdPendIntent
{
	[Activity(Label = "LocUpdPendIntent", MainLauncher = true)]
	public class MainActivity : FragmentActivity,  ISharedPreferencesOnSharedPreferenceChangeListener
	{
		const string Tag = "MainActivity";
		const int RequestPermissionsRequestCode = 34;
		/**
		 * The desired interval for location updates. Inexact. Updates may be more or less frequent.
		 */
		const long UpdateInterval = 60000; // Every 60 seconds.

		/**
		 * The fastest rate for active location updates. Updates will never be more frequent
		 * than this value, but they may be less frequent.
		 */
		const long FastestUpdateInterval = 30000; // Every 30 seconds

		/**
		 * The max time before batched results are delivered by location services. Results may be
		 * delivered sooner than this interval.
		 */
		const long MaxWaitTime = UpdateInterval * 5; // Every 5 minutes.

		/**
		 * Stores parameters for requests to the FusedLocationProviderApi.
		 */
		LocationRequest mLocationRequest;

		/**
		 * Provides access to the Fused Location Provider API.
		 */
		FusedLocationProviderClient mFusedLocationClient;

		// UI Widgets.
		Button mRequestUpdatesButton;
		Button mRemoveUpdatesButton;
		TextView mLocationUpdatesResultView;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.activity_main);

			mRequestUpdatesButton = FindViewById(Resource.Id.request_updates_button) as Button;
			mRemoveUpdatesButton = FindViewById(Resource.Id.remove_updates_button) as Button;
			mLocationUpdatesResultView = FindViewById(Resource.Id.location_updates_result) as TextView;

			mRequestUpdatesButton.Click += (sender, e) => {
				RequestLocationUpdates(null);
			};

			mRemoveUpdatesButton.Click += (sender, e) => {
				RemoveLocationUpdates(null);
			};

			// Check if the user revoked runtime permissions.
			if (!CheckPermissions())
			{
				RequestPermissions();
			}

			mFusedLocationClient = LocationServices.GetFusedLocationProviderClient(this);
			CreateLocationRequest();

		}

		protected override void OnStart()
		{
			base.OnStart();
			PreferenceManager.GetDefaultSharedPreferences(this).RegisterOnSharedPreferenceChangeListener(this);
		}

		protected override void OnResume()
		{
			base.OnResume();
			UpdateButtonsState(Utils.GetRequestingLocationUpdates(this));
			mLocationUpdatesResultView.Text = Utils.GetLocationUpdatesResult(this);
		}

		protected override void OnStop()
		{
			PreferenceManager.GetDefaultSharedPreferences(this).UnregisterOnSharedPreferenceChangeListener(this);
			base.OnStop();
		}

		/**
	     * Sets up the location request. Android has two location request settings:
	     * {@code ACCESS_COARSE_LOCATION} and {@code ACCESS_FINE_LOCATION}. These settings control
	     * the accuracy of the current location. This sample uses ACCESS_FINE_LOCATION, as defined in
	     * the AndroidManifest.xml.
	     * <p/>
	     * When the ACCESS_FINE_LOCATION setting is specified, combined with a fast update
	     * interval (5 seconds), the Fused Location Provider API returns location updates that are
	     * accurate to within a few feet.
	     * <p/>
	     * These settings are appropriate for mapping applications that show real-time location
	     * updates.
	     */
		void CreateLocationRequest()
		{
			mLocationRequest = new LocationRequest();

			// Sets the desired interval for active location updates. This interval is
			// inexact. You may not receive updates at all if no location sources are available, or
			// you may receive them slower than requested. You may also receive updates faster than
			// requested if other applications are requesting location at a faster interval.
			// Note: apps running on "O" devices (regardless of targetSdkVersion) may receive updates
			// less frequently than this interval when the app is no longer in the foreground.
			mLocationRequest.SetInterval(UpdateInterval);

			// Sets the fastest rate for active location updates. This interval is exact, and your
			// application will never receive updates faster than this value.
			mLocationRequest.SetFastestInterval(FastestUpdateInterval);

			mLocationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);

			// Sets the maximum time when batched location updates are delivered. Updates may be
			// delivered sooner than this interval.
			mLocationRequest.SetMaxWaitTime(MaxWaitTime);
		}

		PendingIntent GetPendingIntent()
		{
			// Note: for apps targeting API level 25 ("Nougat") or lower, either
			// PendingIntent.getService() or PendingIntent.getBroadcast() may be used when requesting
			// location updates. For apps targeting API level O, only
			// PendingIntent.getBroadcast() should be used. This is due to the limits placed on services
			// started in the background in "O".

			// TODO(developer): uncomment to use PendingIntent.GetService().
			//        var intent = new Intent(this, typeof(LocationUpdatesIntentService));
			//        intent.SetAction(LocationUpdatesIntentService.ActionProcessUpdate);
			//        return PendingIntent.GetService(this, 0, intent, PendingIntentFlags.UpdateCurrent);

			var intent = new Intent(this, typeof(LocationUpdatesBroadcastReceiver));
			intent.SetAction(LocationUpdatesBroadcastReceiver.ActionProcessUpdates);
			return PendingIntent.GetBroadcast(this, 0, intent, PendingIntentFlags.UpdateCurrent);
		}

		/**
	     * Return the current state of the permissions needed.
	     */
		bool CheckPermissions()
		{
			var permissionState = ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation);
			return permissionState == PermissionChecker.PermissionGranted;
		}

		void RequestPermissions()
		{
			var shouldProvideRationale = ActivityCompat.ShouldShowRequestPermissionRationale(this,
					Manifest.Permission.AccessFineLocation);

			// Provide an additional rationale to the user. This would happen if the user denied the
			// request previously, but didn't check the "Don't ask again" checkbox.
			if (shouldProvideRationale)
			{
				Log.Info(Tag, "Displaying permission rationale to provide additional context.");
				Snackbar.Make(
						FindViewById(Resource.Id.activity_main),
						Resource.String.permission_rationale,
						Snackbar.LengthIndefinite)
				        .SetAction(Resource.String.ok, (obj) => {
							// Request permission
							ActivityCompat.RequestPermissions(this,
									new string[] { Manifest.Permission.AccessFineLocation },
									RequestPermissionsRequestCode);
						})
				        .Show();
			}
			else
			{
				Log.Info(Tag, "Requesting permission");
				// Request permission. It's possible this can be auto answered if device policy
				// sets the permission in a given state or the user denied the permission
				// previously and checked "Never ask again".
				ActivityCompat.RequestPermissions(this,
						new string[] { Manifest.Permission.AccessFineLocation },
						RequestPermissionsRequestCode);
			}
		}

		/**
	     * Callback received when a permissions request has been completed.
	     */
		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			if (requestCode == RequestPermissionsRequestCode)
			{
				if (grantResults.Length <= 0)
				{
					// If user interaction was interrupted, the permission request is cancelled and you
					// receive empty arrays.
					Log.Info(Tag, "User interaction was cancelled.");
				}
				else if (grantResults[0] == Permission.Granted)
				{
					// Permission was granted.
					RequestLocationUpdates(null);
				}
				else
				{
					// Permission denied.

					// Notify the user via a SnackBar that they have rejected a core permission for the
					// app, which makes the Activity useless. In a real app, core permissions would
					// typically be best requested during a welcome-screen flow.

					// Additionally, it is important to remember that a permission might have been
					// rejected without asking the user for permission (device policy or "Never ask
					// again" prompts). Therefore, a user interface affordance is typically implemented
					// when permissions are denied. Otherwise, your app could appear unresponsive to
					// touches or interactions which have required permissions.
					Snackbar.Make(
						FindViewById(Resource.Id.activity_main),
						Resource.String.permission_denied_explanation,
						Snackbar.LengthIndefinite)
						.SetAction(Resource.String.ok, (obj) => {
							// Build intent that displays the App settings screen.
							var intent = new Intent();
							intent.SetAction(Android.Provider.Settings.ActionApplicationDetailsSettings);
							var uri = Uri.FromParts("package", BuildConfig.ApplicationId, null);
							intent.SetData(uri);
							intent.SetFlags(ActivityFlags.NewTask);
							StartActivity(intent);
						})
						.Show();
				}
			}
		}

		public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
		{
			if (key.Equals(Utils.KeyLocationUpdatesResult))
			{
				mLocationUpdatesResultView.Text = Utils.GetLocationUpdatesResult(this);
			}
			else if (key.Equals(Utils.KeyLocationUpdatesRequested))
			{
				UpdateButtonsState(Utils.GetRequestingLocationUpdates(this));
			}
		}

		/**
	     * Handles the Request Updates button and requests start of location updates.
	     */
		public void RequestLocationUpdates(View view)
		{
			try
			{
				Log.Info(Tag, "Starting location updates");
				Utils.SetRequestingLocationUpdates(this, true);
				mFusedLocationClient.RequestLocationUpdates(mLocationRequest, GetPendingIntent());
			}
			catch (SecurityException e)
			{
				Utils.SetRequestingLocationUpdates(this, false);
				e.PrintStackTrace();
			}
		}

		/**
	     * Handles the Remove Updates button, and requests removal of location updates.
	     */
		public void RemoveLocationUpdates(View view)
		{
			Log.Info(Tag, "Removing location updates");
			Utils.SetRequestingLocationUpdates(this, false);
			mFusedLocationClient.RemoveLocationUpdates(GetPendingIntent());
		}

		/**
		 * Ensures that only one button is enabled at any time. The Start Updates button is enabled
		 * if the user is not requesting location updates. The Stop Updates button is enabled if the
		 * user is requesting location updates.
		 */
		void UpdateButtonsState(bool requestingLocationUpdates)
		{
			mRequestUpdatesButton.Enabled = !requestingLocationUpdates;
			mRemoveUpdatesButton.Enabled = requestingLocationUpdates;
		}

	}
}

