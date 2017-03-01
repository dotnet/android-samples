/*
 * Copyright (C) 2012 The Android Open Source Project
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

// C# Port by Atsushi Eno
// Copyright (C) 2012 Xamarin, Inc. (Apache License, Version 2.0 too)

using System;
using Android.App;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;

using Android.OS;

namespace GooglePlayServicesTest
{
	[Activity (Label = "XA GoogleMapV2 Basic Map")]
	public class BasicMapActivity : Android.Support.V4.App.FragmentActivity, IOnMapReadyCallback
	{
		/**
     * Note that this may be null if the Google Play services APK is not available.
     */
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.basic_demo);

			var mapFragment = ((SupportMapFragment)SupportFragmentManager.FindFragmentById (Resource.Id.map));
			mapFragment.GetMapAsync (this);
		}

		/**
     * This is where we can add markers or lines, add listeners or move the camera. In this case, we
     * just add a marker near Africa.
     */
		public void OnMapReady (GoogleMap map)
		{
			map.AddMarker (new MarkerOptions ().SetPosition (new LatLng (0, 0)).SetTitle ("Marker"));
		}	
	}
}

