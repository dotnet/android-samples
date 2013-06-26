/*
 * Copyright 2013 Urban Airship and Contributors
 */
//
// C# Port by Atsushi Eno
// Copyright (C) 2013 Xamarin Inc.
//
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.UrbanAirship.RichPush;
using Java.Lang;
using Xamarin.UrbanAirship;
using Xamarin.UrbanAirship.Push;
using Xamarin.ActionbarSherlockBinding.App;
using Xamarin.UrbanAirship.Utils;
using Xamarin.UrbanAirship.PreferencesAPI;
using ABSActionBar = Xamarin.ActionbarSherlockBinding.App.ActionBar;
using IMenu = Xamarin.ActionbarSherlockBinding.Views.IMenu;
using IMenuItem = Xamarin.ActionbarSherlockBinding.Views.IMenuItem;

namespace Xamarin.Samples.UrbanAirship.RichPush
{
	[Activity (Label = "PushPreferencesActivity")]			
	// ActionBarSherlock does not support the new PreferenceFragment, so we fall back to using
	// deprecated methods. See https://github.com/JakeWharton/ActionBarSherlock/issues/411
	//@SuppressWarnings("deprecation")
		public class PushPreferencesActivity : SherlockPreferenceActivity {

		private UAPreferenceAdapter preferenceAdapter;

		override
			protected void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);

			// Set the actionBar to have up navigation
			ABSActionBar actionBar = SupportActionBar;
			if (actionBar != null) {
				actionBar.SetDisplayOptions (
					ABSActionBar.DisplayHomeAsUp, ABSActionBar.DisplayHomeAsUp);
			}


			AirshipConfigOptions options = UAirship.Shared().AirshipConfigOptions;

			// Only add the push preferences if the pushServiceEnabled is true
			if (options.PushServiceEnabled) {
				this.AddPreferencesFromResource(Resource.Xml.push_preferences);
			}

			// Only add the location preferences if the locationServiceEnabled is true
			if (options.LocationOptions.LocationServiceEnabled) {
				this.AddPreferencesFromResource(Resource.Xml.location_preferences);
			}

			// Display the advanced settings
			if (options.PushServiceEnabled) {
				this.AddPreferencesFromResource(Resource.Xml.advanced_preferences);
			}

			// Creates the UAPreferenceAdapter with the entire preference screen
			preferenceAdapter = new UAPreferenceAdapter(PreferenceScreen);
		}

		override
			public bool OnOptionsItemSelected(IMenuItem item) {
			if (item.ItemId == Android.Resource.Id.Home) {
				OnBackPressed();
				return true;
			}
			return false;
		}

		override
			protected void OnStart() {
			base.OnStart();

			// Activity instrumentation for analytic tracking
			UAirship.Shared().Analytics.ActivityStarted(this);
		}

		override
			protected void OnStop() {
			base.OnStop();

			// Activity instrumentation for analytic tracking
			UAirship.Shared().Analytics.ActivityStopped(this);

			// Apply any changed UA preferences from the preference screen
			preferenceAdapter.ApplyUrbanAirshipPreferences();
		}
	}
}
