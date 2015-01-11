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
using Android.Content.Res;
using Android.Text.Format;
using Java.Util;

namespace PushSample
{
	[Activity (Label = "CustomPreferencesActivity")]
	// This class represents the UI and implementation of the activity enabling users
	// to set Quiet Time preferences.

	public class CustomPreferencesActivity : InstrumentedActivity 
	{
		CheckBox pushEnabled;
		CheckBox soundEnabled;
		CheckBox vibrateEnabled;
		CheckBox quietTimeEnabled;
		CheckBox locationEnabled;
		CheckBox backgroundLocationEnabled;
		CheckBox foregroundLocationEnabled;

		TextView locationEnabledLabel;
		TextView backgroundLocationEnabledLabel;
		TextView foregroundLocationEnabledLabel;

		TimePicker startTime;
		TimePicker endTime;

		PushPreferences pushPrefs = PushManager.Shared().Preferences;
		LocationPreferences locPrefs = UALocationManager.Shared().Preferences;

		private void PushSettingsActive(bool active) {
			soundEnabled.Enabled = active;
			vibrateEnabled.Enabled = active;
		}

		private void QuietTimeSettingsActive(bool active) {
			startTime.Enabled = active;
			endTime.Enabled = active;
		}

		private void BackgroundLocationActive(bool active) {
			backgroundLocationEnabled.Enabled = active;
		}

		private void ForegroundLocationActive (bool active) {
			foregroundLocationEnabled.Enabled = active;
		}

		protected override void OnCreate(Bundle icicle) {
			base.OnCreate(icicle);

			Window w = Window;
			w.RequestFeature (WindowFeatures.LeftIcon);
			SetContentView(Resource.Layout.push_preferences_dialog);

			pushEnabled = (CheckBox) FindViewById(Resource.Id.push_enabled);
			soundEnabled = (CheckBox) FindViewById(Resource.Id.sound_enabled);
			vibrateEnabled = (CheckBox) FindViewById(Resource.Id.vibrate_enabled);
			quietTimeEnabled = (CheckBox) FindViewById(Resource.Id.quiet_time_enabled);
			locationEnabled = (CheckBox) FindViewById(Resource.Id.location_enabled);
			backgroundLocationEnabled = (CheckBox) FindViewById(Resource.Id.background_location_enabled);
			foregroundLocationEnabled = (CheckBox) FindViewById(Resource.Id.foreground_location_enabled);
			locationEnabledLabel = (TextView) FindViewById(Resource.Id.location_enabled_label);
			backgroundLocationEnabledLabel = (TextView) FindViewById(Resource.Id.background_location_enabled_label);
			foregroundLocationEnabledLabel = (TextView) FindViewById(Resource.Id.foreground_location_enabled_label);

			startTime = (TimePicker) FindViewById(Resource.Id.start_time);
			endTime = (TimePicker) FindViewById(Resource.Id.end_time);

			startTime.SetIs24HourView (new Java.Lang.Boolean (DateFormat.Is24HourFormat(this)));
			endTime.SetIs24HourView (new Java.Lang.Boolean (DateFormat.Is24HourFormat(this)));

			pushEnabled.Click += (v, e) => {
				PushSettingsActive(((CheckBox) v).Checked);
			};
			quietTimeEnabled.Click += (v, e) => {
				QuietTimeSettingsActive(((CheckBox)v).Checked);
			};

			locationEnabled.Click += (v, e) => {
				BackgroundLocationActive(((CheckBox)v).Checked);
				ForegroundLocationActive(((CheckBox)v).Checked);
			};

		}

		// When the activity starts, we need to fetch and display the user's current
		// Push preferences in the view, if applicable.
		public override void OnStart() {
			base.OnStart();

			bool isPushEnabled = pushPrefs.IsPushEnabled;
			pushEnabled.Checked = isPushEnabled;
			soundEnabled.Checked = pushPrefs.SoundEnabled;
			vibrateEnabled.Checked = pushPrefs.VibrateEnabled;
			PushSettingsActive(isPushEnabled);

			bool isQuietTimeEnabled = pushPrefs.QuietTimeEnabled;
			quietTimeEnabled.Checked = isQuietTimeEnabled;
			QuietTimeSettingsActive(isQuietTimeEnabled);

			if (!UAirship.Shared().AirshipConfigOptions.LocationOptions.LocationServiceEnabled) {
				locationEnabled.Visibility = ViewStates.Gone;
				backgroundLocationEnabled.Visibility = ViewStates.Gone;
				foregroundLocationEnabled.Visibility = ViewStates.Gone;
				locationEnabledLabel.Visibility = ViewStates.Gone;
				backgroundLocationEnabledLabel.Visibility = ViewStates.Gone;
				foregroundLocationEnabledLabel.Visibility = ViewStates.Gone;

			} else {
				locationEnabled.Checked = locPrefs.IsLocationEnabled;
				backgroundLocationEnabled.Checked = locPrefs.IsBackgroundLocationEnabled;
				foregroundLocationEnabled.Checked = locPrefs.IsForegroundLocationEnabled;
			}

			//this will be null if a quiet time interval hasn't been set
			Date[] interval = pushPrefs.GetQuietTimeInterval ();
			if(interval != null) {
				startTime.CurrentHour = new Java.Lang.Integer (interval[0].Hours);
				startTime.CurrentMinute = new Java.Lang.Integer (interval[0].Minutes);
				endTime.CurrentHour = new Java.Lang.Integer (interval[1].Hours);
				endTime.CurrentMinute = new Java.Lang.Integer (interval[1].Minutes);
			}
		}

		// When the activity is closed, save the user's Push preferences
		public override void OnStop() {
			base.OnStop();

			bool IsPushEnabledInActivity = pushEnabled.Checked;
			bool IsQuietTimeEnabledInActivity = quietTimeEnabled.Checked;

			if(IsPushEnabledInActivity) {
				PushManager.EnablePush();
			}
			else {
				PushManager.DisablePush();
			}

			pushPrefs.SoundEnabled = soundEnabled.Checked;
			pushPrefs.VibrateEnabled = vibrateEnabled.Checked;

			pushPrefs.QuietTimeEnabled = IsQuietTimeEnabledInActivity;

			if(IsQuietTimeEnabledInActivity) {

				// Grab the start date.
				Calendar cal = Calendar.Instance;
				cal.Set(Calendar.HourOfDay, (int) startTime.CurrentHour);
				cal.Set(Calendar.Minute, (int) startTime.CurrentMinute);
				Date startDate = cal.Time;

				// Prepare the end date.
				cal = Calendar.Instance;
				cal.Set(Calendar.HourOfDay, (int) endTime.CurrentHour);
				cal.Set(Calendar.Minute, (int) endTime.CurrentMinute);
				Date endDate = cal.Time;

				pushPrefs.SetQuietTimeInterval (startDate, endDate);
			}

			this.HandleLocation();

		}

		private void HandleLocation() {
			if (!UAirship.Shared().AirshipConfigOptions.LocationOptions.LocationServiceEnabled) {
				return;
			}
			bool isLocationEnabledInActivity = locationEnabled.Checked;
			bool isBackgroundLocationEnabledInActivity = backgroundLocationEnabled.Checked;
			bool isForegroundLocationEnabledInActivity = foregroundLocationEnabled.Checked;

			// Set the location enable preference first because it will be used
			// in the logic to enable/disable background and foreground locations.
			if (isLocationEnabledInActivity) {
				UALocationManager.EnableLocation();
			} else {
				UALocationManager.DisableLocation();
			}
			HandleBackgroundLocationPreference(isBackgroundLocationEnabledInActivity);
			HandleForegroundLocationPreference(isForegroundLocationEnabledInActivity);
		}

		private void HandleBackgroundLocationPreference(bool backgroundLocationEnabled) {
			if (backgroundLocationEnabled) {
				UALocationManager.EnableBackgroundLocation();
			} else {
				UALocationManager.DisableBackgroundLocation();
			}
		}

		private void HandleForegroundLocationPreference(bool foregroundLocationEnabled) {
			if (foregroundLocationEnabled) {
				UALocationManager.EnableForegroundLocation();
			} else {
				UALocationManager.DisableForegroundLocation();
			}
		}
		protected void OnConfigurationChanged(Configuration newConfig) {
			base.OnConfigurationChanged(newConfig);
			// DO NOT REMOVE, just having it here seems to fix a weird issue with
			// Time picker where the fields would go blank on rotation.
		}

	}
}
