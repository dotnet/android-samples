/*
Copyright 2009-2011 Urban Airship Inc. All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice,
this list of conditions and the following disclaimer in the documentation
and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE URBAN AIRSHIP INC ``AS IS'' AND ANY EXPRESS OR
IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO
EVENT SHALL URBAN AIRSHIP INC OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE
OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.UrbanAirship;
using Xamarin.UrbanAirship.AnalyticsAPI;
using Xamarin.UrbanAirship.Locations;
using Xamarin.UrbanAirship.Push;

namespace PushSample
{
	[Activity (Label = "PushSample", MainLauncher = true)]	
	public class MainActivity : InstrumentedActivity
	{
		Button locationButton;

		IntentFilter boundServiceFilter;
		IntentFilter apidUpdateFilter;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.main);

			locationButton = (Button)FindViewById (Resource.Id.location_button);
			locationButton.Click += delegate {
				StartActivity (new Intent (BaseContext, typeof(LocationActivity)));
			};

			// Set up custom preference screen style button
			Button customPreferencesButton = (Button)FindViewById (Resource.Id.push_custom_preferences_button);
			customPreferencesButton.Click += delegate {
				StartActivity (new Intent (BaseContext, typeof(CustomPreferencesActivity)));
			};

			// Set up android built-in preference screen style button
			Button preferencesButton = (Button)FindViewById (Resource.Id.push_preferences_button);
			preferencesButton.Click += delegate {
				StartActivity (new Intent (BaseContext, typeof(PreferencesActivity)));
			};

			boundServiceFilter = new IntentFilter ();
			boundServiceFilter.AddAction (UALocationManager.ActionLocationServiceBound);
			boundServiceFilter.AddAction (UALocationManager.ActionLocationServiceUnbound);

			apidUpdateFilter = new IntentFilter ();
			apidUpdateFilter.AddAction (UAirship.PackageName + IntentReceiver.APID_UPDATED_ACTION_SUFFIX);
		}

		protected override void OnResume ()
		{
			base.OnResume ();

			// OPTIONAL! The following block of code removes all notifications from the status bar.
			NotificationManager notificationManager = (NotificationManager)GetSystemService (Context.NotificationService);
			notificationManager.CancelAll ();

			HandleLocationButton ();

			RegisterReceiver (boundServiceReceiver, boundServiceFilter);
			RegisterReceiver (apidUpdateReceiver, apidUpdateFilter);
			UpdateApidField ();
		}

		private void HandleLocationButton ()
		{
			if (UALocationManager.IsServiceBound) {
				Logger.Info ("LocationService is bound to MainActivity");
				locationButton.Enabled = (true);
			} else {
				Logger.Info ("LocationService is not bound to MainActivity");
				locationButton.Enabled = (false);
			}
		}

		protected override void OnPause ()
		{
			base.OnPause ();
			try {
				UnregisterReceiver (boundServiceReceiver);
				UnregisterReceiver (apidUpdateReceiver);
			} catch (Java.Lang.IllegalArgumentException e) {
				Logger.Error (e.Message);
			}
		}

		class DelegateBroadcastReceiver : BroadcastReceiver
		{
			Action<Context,Intent> onReceive;

			public DelegateBroadcastReceiver (Action<Context,Intent> onReceive)
			{
				this.onReceive = onReceive;
			}

			public override void OnReceive (Context context, Intent intent)
			{
				onReceive (context, intent);
			}
		}

		public MainActivity ()
		{
			boundServiceReceiver = new DelegateBroadcastReceiver (BoundServiceReceiver_OnReceive);
			apidUpdateReceiver = new DelegateBroadcastReceiver (ApidUpdateReceiver_OnReceive);
		}

		private BroadcastReceiver boundServiceReceiver;

		void BoundServiceReceiver_OnReceive (Context context, Intent intent)
		{
			if (UALocationManager.ActionLocationServiceBound == intent.Action) {
				locationButton.Enabled = true;
			} else {
				locationButton.Enabled = false;
			}
		}

		private BroadcastReceiver apidUpdateReceiver;

		void ApidUpdateReceiver_OnReceive (Context context, Intent intent)
		{
			UpdateApidField ();
		}

		private void UpdateApidField ()
		{
			String apidString = PushManager.Shared ().APID;
			if (!PushManager.Shared ().Preferences.IsPushEnabled || apidString == null) {
				apidString = "";
			}

			// fill in apid text
			EditText apidTextField = (EditText)FindViewById (Resource.Id.apidText);
			if (!apidString.Equals (apidTextField.Text)) {
				apidTextField.Text = (apidString);
			}
		}
	}
}
