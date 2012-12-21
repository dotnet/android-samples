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
	[Activity (Label = "Mono GoogleMapV2 Basic Map")]
	public class BasicMapActivity : Android.Support.V4.App.FragmentActivity
	{
		/**
     * Note that this may be null if the Google Play services APK is not available.
     */
		private GoogleMap mMap;
		
		protected override void OnCreate (Bundle savedInstanceState) {
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.basic_demo);
			SetUpMapIfNeeded ();
		}
		
		protected override void OnResume() 
		{
			base.OnResume ();
			SetUpMapIfNeeded ();
		}
		
		/**
     * Sets up the map if it is possible to do so (i.e., the Google Play services APK is correctly
     * installed) and the map has not already been instantiated.. This will ensure that we only ever
     * call {@link #setUpMap()} once when {@link #mMap} is not null.
     * <p>
     * If it isn't installed {@link SupportMapFragment} (and
     * {@link com.google.android.gms.maps.MapView
     * MapView}) will show a prompt for the user to install/update the Google Play services APK on
     * their device.
     * <p>
     * A user can return to this Activity after following the prompt and correctly
     * installing/updating/enabling the Google Play services. Since the Activity may not have been
     * completely destroyed during this process (it is likely that it would only be stopped or
     * paused), {@link #onCreate(Bundle)} may not be called again so we should call this method in
     * {@link #onResume()} to guarantee that it will be called.
     */
		private void SetUpMapIfNeeded() 
		{
			// Do a null check to confirm that we have not already instantiated the map.
			if (mMap == null) {
				// Try to obtain the map from the SupportMapFragment.
				mMap = ((SupportMapFragment) SupportFragmentManager.FindFragmentById (Resource.Id.map)).Map;
				// Check if we were successful in obtaining the map.
				if (mMap != null) {
					SetUpMap ();
				}
			}
		}
		
		/**
     * This is where we can add markers or lines, add listeners or move the camera. In this case, we
     * just add a marker near Africa.
     * <p>
     * This should only be called once and when we are sure that {@link #mMap} is not null.
     */
		private void SetUpMap() {
			mMap.AddMarker (new MarkerOptions ().SetPosition (new LatLng (0, 0)).SetTitle ("Marker"));
		}	
	}
}

