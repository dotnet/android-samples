using System;
using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.OS;
using Android.Preferences;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace GCMSample
{
	[Activity (MainLauncher = true)]
	public class MainActivity : Activity
	{
		const int PLAY_SERVICES_RESOLUTION_REQUEST = 9000;
		const string TAG = "MainActivity";

		BroadcastReceiver mRegistrationBroadcastReceiver;
		ProgressBar mRegistrationProgressBar;
		TextView mInformationTextView;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.activity_main);

			mRegistrationProgressBar = FindViewById<ProgressBar> (Resource.Id.registrationProgressBar);
			mRegistrationBroadcastReceiver = new BroadcastReceiver ();
			mRegistrationBroadcastReceiver.OnReceiveImpl = (context, intent) => {
				mRegistrationProgressBar.Visibility = ViewStates.Gone;
				var sharedPreferences =
					PreferenceManager.GetDefaultSharedPreferences (context);
				var sentToken = sharedPreferences.GetBoolean (QuickstartPreferences.SENT_TOKEN_TO_SERVER, false);
				if (sentToken) {
					mInformationTextView.Text = GetString (Resource.String.gcm_send_message);
				} else {
					mInformationTextView.Text = GetString (Resource.String.token_error_message);
				}
			};
			mInformationTextView = FindViewById<TextView> (Resource.Id.informationTextView);

			if (CheckPlayServices ()) {
				// Start IntentService to register this application with GCM.
				var intent = new Intent (this, typeof(RegistrationIntentService));
				StartService (intent);
			}
		}

		protected override void OnResume ()
		{
			base.OnResume ();
			LocalBroadcastManager.GetInstance (this).RegisterReceiver (mRegistrationBroadcastReceiver,
				new IntentFilter (QuickstartPreferences.REGISTRATION_COMPLETE));
		}

		protected override void OnPause ()
		{
			LocalBroadcastManager.GetInstance (this).UnregisterReceiver (mRegistrationBroadcastReceiver);
			base.OnPause ();
		}

		bool CheckPlayServices ()
		{
			int resultCode = GooglePlayServicesUtil.IsGooglePlayServicesAvailable (this);
			if (resultCode != ConnectionResult.Success) {
				if (GooglePlayServicesUtil.IsUserRecoverableError (resultCode)) {
					GooglePlayServicesUtil.GetErrorDialog (resultCode, this,
						PLAY_SERVICES_RESOLUTION_REQUEST).Show ();
				} else {
					Log.Info (TAG, "This device is not supported.");
					Finish ();
				}
				return false;
			}
			return true;
		}

		class BroadcastReceiver : Android.Content.BroadcastReceiver
		{
			public Action <Context, Intent> OnReceiveImpl {get;set;}
			public override void OnReceive (Context context, Intent intent)
			{
				OnReceiveImpl (context, intent);
			}
		}
	}
}


