/*
 * Copyright (C) 2009 The Android Open Source Project
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

//
// C# port Coyright (C) 2012 Xamarin Inc.
//

using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.Gestures;
using Android.Runtime;
using Android.OS;
using Android.Views;
using Android.Widget;
using Java.Interop;

using Object = Java.Lang.Object;
using Path = System.IO.Path;
using Environment = Android.OS.Environment;
using Java.Interop;

namespace GestureBuilder
{
	[Activity (Label = "@string/label_create_gesture")]
	public class CreateGestureActivity : Activity
	{
		public CreateGestureActivity ()
		{
		}
		const float LENGTH_THRESHOLD = 120.0f;

		Gesture mGesture;
		View mDoneButton;

		protected override void OnCreate (Bundle savedInstanceState) 
		{
			base.OnCreate (savedInstanceState);
        
			SetContentView (Resource.Layout.create_gesture);

			mDoneButton = FindViewById(Resource.Id.done);

			GestureOverlayView overlay = (GestureOverlayView) FindViewById (Resource.Id.gestures_overlay);
			overlay.AddOnGestureListener (new GesturesProcessor (this));
		}

		protected override void OnSaveInstanceState (Bundle outState) 
		{
			base.OnSaveInstanceState (outState);
        
			if (mGesture != null)
				outState.PutParcelable ("gesture", mGesture);
		}

		protected override void OnRestoreInstanceState (Bundle savedInstanceState) 
		{
			base.OnRestoreInstanceState (savedInstanceState);
        
			mGesture = (Gesture) savedInstanceState.GetParcelable ("gesture");
			if (mGesture != null) {
				var overlay = (GestureOverlayView) FindViewById (Resource.Id.gestures_overlay);
				overlay.Post (() => overlay.Gesture = mGesture);

				mDoneButton.Enabled = true;
			}
		}

		[Export ("addGesture")]
		public void AddGesture (View v) 
		{
			if (mGesture != null) {
				TextView input = (TextView) FindViewById (Resource.Id.gesture_name);
				var name = input.TextFormatted;
				if (name.Length () == 0) {
					input.Error = GetString (Resource.String.error_missing_name);
					return;
				}

				GestureLibrary store = GestureBuilderActivity.Store;
				store.AddGesture (name.ToString (), mGesture);
				store.Save ();

				SetResult (Result.Ok);

				string path = Path.Combine (Environment.ExternalStorageDirectory.AbsolutePath, "gestures");
				Toast.MakeText (this, GetString (Resource.String.save_success, path), ToastLength.Long).Show ();
			} else {
				SetResult (Result.Canceled);
			}

			Finish ();
		}
    
		[Export ("cancelGesture")]
		public void CancelGesture (View v) 
		{
			SetResult (Result.Canceled);
			Finish ();
		}
    
		private class GesturesProcessor : Object, GestureOverlayView.IOnGestureListener
		{
			CreateGestureActivity parent;
			public GesturesProcessor (CreateGestureActivity parent)
			{
				this.parent = parent;
			}
			
			public void OnGestureStarted (GestureOverlayView overlay, MotionEvent evt) 
			{
				parent.mDoneButton.Enabled = false;
				parent.mGesture = null;
			}

			public void OnGesture (GestureOverlayView overlay, MotionEvent evt)
			{
			}

			public void OnGestureEnded (GestureOverlayView overlay, MotionEvent evt) 
			{
				parent.mGesture = overlay.Gesture;
				if (parent.mGesture.Length < LENGTH_THRESHOLD)
					overlay.Clear (false);
				parent.mDoneButton.Enabled = true;
			}

			public void OnGestureCancelled (GestureOverlayView overlay, MotionEvent evt) 
			{
			}
		}
	}
}
