/*
 * Copyright (C) 2007 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Hardware;
using Android.OS;
using Android.Views;
using Android.GoogleMaps;

// Example of how to use a MapsView in conjunction with the SensorManager
namespace MonoDroid.Samples.MapsDemo
{
	[Activity (Label = "MapView and Compass")]
	public class MapViewCompassDemo : MapActivity
	{
		const String TAG = "MapViewCompassDemo";

		private SensorManager sensor_manager;
		private RotateView rotate_view;
		private MyLocationOverlay location_overlay;

		protected override void OnCreate (Bundle savedInstanceState) 
		{
			base.OnCreate (savedInstanceState);

			// Get a reference to the sensor manager
			sensor_manager = (SensorManager) GetSystemService (Context.SensorService);

			// Create our view
			var map_view = new MapView (this, "MapViewCompassDemo_DummyAPIKey");

			rotate_view = new RotateView (this);
			rotate_view.AddView (map_view);

			SetContentView (rotate_view);
			
			// Create the location overlay
			location_overlay = new MyLocationOverlay (this, map_view);
			location_overlay.RunOnFirstFix (delegate {
				map_view.Controller.AnimateTo (location_overlay.MyLocation);
			});
			map_view.Overlays.Add (location_overlay);

			map_view.Controller.SetZoom(18);
			map_view.Clickable = true;
			map_view.Enabled = true;
		}

		protected override void OnResume () 
		{
			base.OnResume ();

			var orientSensor = sensor_manager.GetDefaultSensor (SensorType.Orientation);
			sensor_manager.RegisterListener (rotate_view, orientSensor, SensorDelay.Ui);
			
			location_overlay.EnableMyLocation ();
		}

		protected override void OnStop () 
		{
			sensor_manager.UnregisterListener (rotate_view);
			location_overlay.DisableMyLocation ();

			base.OnStop ();
		}

		protected override bool IsRouteDisplayed {
			get { return false; }
		}
	}
}
