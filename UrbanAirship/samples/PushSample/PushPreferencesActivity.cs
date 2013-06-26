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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.UrbanAirship;
using Xamarin.UrbanAirship.AnalyticsAPI;
using Xamarin.UrbanAirship.Locations;
using Xamarin.UrbanAirship.Push;
using Xamarin.UrbanAirship.PreferencesAPI;

namespace PushSample
{
	// This class represents the UI and implementation of the activity enabling users
	// to set Quiet Time preferences.
	[Activity (Label = "PushPreferencesActivity")]
	public class PreferencesActivity : PreferenceActivity
	{
		private UAPreferenceAdapter _preferenceAdapter;
		UAPreferenceAdapter preferenceAdapter {
			get {
				if (_preferenceAdapter == null)					
					// Creates the UAPreferenceAdapter with the entire preference screen
					_preferenceAdapter = new UAPreferenceAdapter(PreferenceScreen);
				return _preferenceAdapter;
			}
		}

		protected void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			AirshipConfigOptions options = UAirship.Shared ().AirshipConfigOptions;

			// Only add the push preferences if the pushServiceEnabled is true
			if (options.PushServiceEnabled) {
				this.AddPreferencesFromResource (Resource.Xml.push_preferences);
			}

			// Only add the location preferences if the locationServiceEnabled is true
			if (options.LocationOptions.LocationServiceEnabled) {
				this.AddPreferencesFromResource (Resource.Xml.location_preferences);
			}

			// Display the advanced settings
			if (options.PushServiceEnabled) {
				this.AddPreferencesFromResource (Resource.Xml.advanced_preferences);
			}
		}

		protected override void OnStart ()
		{
			base.OnStart ();

			// Activity instrumentation for analytic tracking
			UAirship.Shared ().Analytics.ActivityStarted (this);
		}

		protected override void OnStop ()
		{
			base.OnStop ();

			// Activity instrumentation for analytic tracking
			UAirship.Shared ().Analytics.ActivityStopped (this);

			// Apply any changed UA preferences from the preference screen
			preferenceAdapter.ApplyUrbanAirshipPreferences ();
		}
	}
}