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
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;

namespace GooglePlayServicesTest
{
	[Activity (Label = "Mono GoogleMapV2 MarkerDemo")]
	public class MarkerDemoActivity : Android.Support.V4.App.FragmentActivity,
		GoogleMap.IOnMarkerClickListener, GoogleMap.IOnInfoWindowClickListener, GoogleMap.IOnMarkerDragListener 
	{
		static readonly LatLng BRISBANE = new LatLng(-27.47093, 153.0235);
		static readonly LatLng MELBOURNE = new LatLng(-37.81319, 144.96298);
		static readonly LatLng SYDNEY = new LatLng(-33.87365, 151.20689);
		static readonly LatLng ADELAIDE = new LatLng(-34.92873, 138.59995);
		static readonly LatLng PERTH = new LatLng(-31.952854, 115.857342);
		
		/** Demonstrates customizing the info window and/or its contents. */
		class CustomInfoWindowAdapter : Java.Lang.Object, GoogleMap.IInfoWindowAdapter {
			MarkerDemoActivity parent;
			private readonly RadioGroup mOptions;
			
			// These a both viewgroups containing an ImageView with id "badge" and two TextViews with id
			// "title" and "snippet".
			private readonly View mWindow;
			private readonly View mContents;
			
			internal CustomInfoWindowAdapter (MarkerDemoActivity parent) 
			{
				mWindow = parent.LayoutInflater.Inflate (Resource.Layout.custom_info_window, null);
				mContents = parent.LayoutInflater.Inflate (Resource.Layout.custom_info_contents, null);
				mOptions = (RadioGroup) parent.FindViewById (Resource.Id.custom_info_window_options);
			}
			
			public View GetInfoWindow (Marker marker) {
				if (mOptions.CheckedRadioButtonId != Resource.Id.custom_info_window) {
					// This means that getInfoContents will be called.
					return null;
				}
				Render(marker, mWindow);
				return mWindow;
			}
			
			public View GetInfoContents (Marker marker) {
				if (mOptions.CheckedRadioButtonId != Resource.Id.custom_info_contents) {
					// This means that the default info contents will be used.
					return null;
				}
				Render (marker, mContents);
				return mContents;
			}
			
			private void Render (Marker marker, View view) {
				int badge;
				// Use the equals() method on a Marker to check for equals.  Do not use ==.
				if (marker.Equals(parent.mBrisbane)) {
					badge = Resource.Drawable.badge_qld;
				} else if (marker.Equals(parent.mAdelaide)) {
					badge = Resource.Drawable.badge_sa;
				} else if (marker.Equals(parent.mSydney)) {
					badge = Resource.Drawable.badge_nsw;
				} else if (marker.Equals(parent.mMelbourne)) {
					badge = Resource.Drawable.badge_victoria;
				} else if (marker.Equals(parent.mPerth)) {
					badge = Resource.Drawable.badge_wa;
				} else {
					// Passing 0 to setImageResource will clear the image view.
					badge = 0;
				}
				((ImageView) view.FindViewById (Resource.Id.badge)).SetImageResource (badge);
				
				String title = marker.Title;
				TextView titleUi = ((TextView) view.FindViewById (Resource.Id.title));
				if (title != null) {
					// Spannable string allows us to edit the formatting of the text.
					SpannableString titleText = new SpannableString (title);
					SpanTypes st = (SpanTypes) 0;
					// FIXME: this somehow rejects to compile
					//titleText.SetSpan (new ForegroundColorSpan(Color.Red), 0, titleText.Length, st);
					titleUi.TextFormatted = (titleText);
				} else {
					titleUi.Text = ("");
				}
				
				String snippet = marker.Snippet;
				TextView snippetUi = ((TextView) view.FindViewById(Resource.Id.snippet));
				if (snippet != null) {
					SpannableString snippetText = new SpannableString(snippet);
					snippetText.SetSpan(new ForegroundColorSpan(Color.Magenta), 0, 10, 0);
					snippetText.SetSpan(new ForegroundColorSpan(Color.Blue), 12, 21, 0);
					snippetUi.TextFormatted = (snippetText);
				} else {
					snippetUi.Text = ("");
				}
			}
		}
		
		private GoogleMap mMap;
		
		private Marker mPerth;
		private Marker mSydney;
		private Marker mBrisbane;
		private Marker mAdelaide;
		private Marker mMelbourne;
		private TextView mTopText;
		
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.marker_demo);
			
			mTopText = (TextView) FindViewById (Resource.Id.top_text);
			
			SetUpMapIfNeeded ();
		}
		
		protected override void OnResume ()
		{
			base.OnResume ();
			SetUpMapIfNeeded ();
		}
		
		private void SetUpMapIfNeeded () 
		{
			// Do a null check to confirm that we have not already instantiated the map.
			if (mMap == null) {
				// Try to obtain the map from the SupportMapFragment.
				mMap = ((SupportMapFragment) SupportFragmentManager.FindFragmentById(Resource.Id.map)).Map;
				// Check if we were successful in obtaining the map.
				if (mMap != null) {
					SetUpMap ();
				}
			}
		}
		
		class GlobalLayoutListener : Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener
		{
			Action on_global_layout;
			public GlobalLayoutListener (Action onGlobalLayout)
			{
				on_global_layout = onGlobalLayout;
			}

			public void OnGlobalLayout ()
			{
				on_global_layout ();
			}
		}
		
		private void SetUpMap () 
		{
			// Hide the zoom controls as the button panel will cover it.
			mMap.UiSettings.ZoomControlsEnabled = false;
			
			// Add lots of markers to the map.
			AddMarkersToMap();
			
			// Setting an info window adapter allows us to change the both the contents and look of the
			// info window.
			mMap.SetInfoWindowAdapter (new CustomInfoWindowAdapter (this));
			
			// Set listeners for marker events.  See the bottom of this class for their behavior.
			mMap.SetOnMarkerClickListener(this);
			mMap.SetOnInfoWindowClickListener(this);
			mMap.SetOnMarkerDragListener(this);

			// Pan to see all markers in view.
			// Cannot zoom to bounds until the map has a size.
			View mapView = SupportFragmentManager.FindFragmentById (Resource.Id.map).View;
			if (mapView.ViewTreeObserver.IsAlive) {
				GlobalLayoutListener gll = null;
				gll = new GlobalLayoutListener (
					delegate {
						LatLngBounds bounds = new LatLngBounds.Builder()
							.Include(PERTH)
								.Include(SYDNEY)
								.Include(ADELAIDE)
								.Include(BRISBANE)
								.Include(MELBOURNE)
								.Build ();
						
						/*if (Build.VERSION.SdkInt < Build.VERSION_CODES.JellyBean)*/ {
							mapView.ViewTreeObserver.RemoveGlobalOnLayoutListener (gll);
						} /* else {
							mapView.ViewTreeObserver.RemoveOnGlobalLayoutListener (this);
						}*/
						mMap.MoveCamera (CameraUpdateFactory.NewLatLngBounds (bounds, 50));
				});
				mapView.ViewTreeObserver.AddOnGlobalLayoutListener (gll);
			}
		}
		
		private void AddMarkersToMap () 
		{
			// Uses a colored icon.
			mBrisbane = mMap.AddMarker(new MarkerOptions()
			                           .SetPosition(BRISBANE)
			                           .SetTitle("Brisbane")
			                           .SetSnippet("Population: 2,074,200")
			                           .SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueAzure)));
			
			// Uses a custom icon.
			mSydney = mMap.AddMarker(new MarkerOptions()
			                         .SetPosition(SYDNEY)
			                         .SetTitle("Sydney")
			                         .SetSnippet("Population: 4,627,300")
			                         .SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.arrow)));
			
			// Creates a draggable marker. Long press to drag.
			mMelbourne = mMap.AddMarker(new MarkerOptions()
			                            .SetPosition(MELBOURNE)
			                            .SetTitle("Melbourne")
			                            .SetSnippet("Population: 4,137,400")
			                            .Draggable(true));
			
			// A few more markers for good measure.
			mPerth = mMap.AddMarker(new MarkerOptions()
			                        .SetPosition(PERTH)
			                        .SetTitle("Perth")
			                        .SetSnippet("Population: 1,738,800"));
			mAdelaide = mMap.AddMarker(new MarkerOptions()
			                           .SetPosition(ADELAIDE)
			                           .SetTitle("Adelaide")
			                           .SetSnippet("Population: 1,213,000"));
			
			// Creates a marker rainbow demonstrating how to create default marker icons of different
			// hues (colors).
			int numMarkersInRainbow = 12;
			for (int i = 0; i < numMarkersInRainbow; i++) {
				mMap.AddMarker(new MarkerOptions()
				               .SetPosition(new LatLng(
					-30 + 10 * Math.Sin(i * Math.PI / (numMarkersInRainbow - 1)),
					135 - 10 * Math.Cos(i * Math.PI / (numMarkersInRainbow - 1))))
				               .SetTitle("Marker " + i)
				               .SetIcon(BitmapDescriptorFactory.DefaultMarker(i * 360 / numMarkersInRainbow)));
			}
		}
		
		private bool CheckReady () 
		{
			if (mMap == null) {
				Toast.MakeText(this, Resource.String.map_not_ready, ToastLength.Short).Show();
				return false;
			}
			return true;
		}
		
		/** Called when the Clear button is clicked. */
		public void OnClearMap (View view) 
		{
			if (!CheckReady()) {
				return;
			}
			mMap.Clear();
		}
		
		/** Called when the Reset button is clicked. */
		public void OnResetMap(View view) 
		{
			if (!CheckReady()) {
				return;
			}
			// Clear the map because we don't want duplicates of the markers.
			mMap.Clear();
			AddMarkersToMap();
		}

		class Runnable : Java.Lang.Object, Java.Lang.IRunnable
		{
			Action run;
			public Runnable (Action run)
			{
				this.run = run;
			}

			public void Run ()
			{
				run ();
			}
		}
		
		//
		// Marker related listeners.
		//
		
		public bool OnMarkerClick (Marker marker)
		{
			// This causes the marker at Perth to bounce into position when it is clicked.
			if (marker.Equals(mPerth)) {
				Handler handler = new Handler ();
				long start = SystemClock.UptimeMillis ();
				Projection proj = mMap.Projection;
				Point startPoint = proj.ToScreenLocation(PERTH);
				startPoint.Offset(0, -100);
				LatLng startLatLng = proj.FromScreenLocation(startPoint);
				long duration = 1500;
				
				IInterpolator interpolator = new BounceInterpolator();

				Runnable run = null;
				run = new Runnable (delegate {
						long elapsed = SystemClock.UptimeMillis () - start;
						float t = interpolator.GetInterpolation ((float) elapsed / duration);
						double lng = t * PERTH.Longitude + (1 - t) * startLatLng.Longitude;
						double lat = t * PERTH.Latitude + (1 - t) * startLatLng.Latitude;
						marker.Position = (new LatLng(lat, lng));
						
						if (t < 1.0) {
							// Post again 16ms later.
							handler.PostDelayed(run, 16);
						}
				});
				handler.Post(run);
			}
			// We return false to indicate that we have not consumed the event and that we wish
			// for the default behavior to occur (which is for the camera to move such that the
			// marker is centered and for the marker's info window to open, if it has one).
			return false;
		}
		
		public void OnInfoWindowClick (Marker marker) {
			Toast.MakeText(BaseContext, "Click Info Window", ToastLength.Short).Show();
		}
		
		public void OnMarkerDragStart (Marker marker) {
			mTopText.Text = ("onMarkerDragStart");
		}
		
		public void OnMarkerDragEnd (Marker marker) {
			mTopText.Text = ("onMarkerDragEnd");
		}
		
		public void OnMarkerDrag(Marker marker) {
			mTopText.Text = ("onMarkerDrag.  Current Position: " + marker.Position);
		}
	}
}

