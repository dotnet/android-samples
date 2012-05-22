/*
 * Copyright (C) 2011 The Android Open Source Project
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
using System.Collections.Generic;
using Android.Graphics;
using Java.IO;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Camera = Android.Hardware.Camera;

namespace com.example.monodroid.hcgallery
{
	public class CameraFragment : Fragment
	{

		private Preview mPreview;
		Camera mCamera;
		int mNumberOfCameras;
		int mCameraCurrentlyLocked;
		
		// The first rear facing camera
		int mDefaultCameraId;

		public override void OnCreate (Bundle savedInstanceState) 
		{
			base.OnCreate (savedInstanceState);
			
			
			// Create a RelativeLayout container that will hold a SurfaceView,
			// and set it as the content of our activity.
			mPreview = new Preview (this.Activity);
			
			// Find the total number of cameras available
			mNumberOfCameras = Camera.NumberOfCameras;
			
			// Find the ID of the default camera
			Camera.CameraInfo cameraInfo = new Camera.CameraInfo ();
			for (int i = 0; i < mNumberOfCameras; i++) {
				Camera.GetCameraInfo (i, cameraInfo);
				if (cameraInfo.Facing == Camera.CameraInfo.CameraFacingBack) {
					mDefaultCameraId = i;
				}
			}
			SetHasOptionsMenu (mNumberOfCameras > 1);
		}

		public override void OnActivityCreated (Bundle savedInstanceState) {
			base.OnActivityCreated (savedInstanceState);
			// Add an up arrow to the "home" button, indicating that the button will go "up"
			// one activity in the app's Activity heirarchy.
			// Calls to getActionBar() aren't guaranteed to return the ActionBar when called
			// from within the Fragment's onCreate method, because the Window's decor hasn't been
			// initialized yet.  Either call for the ActionBar reference in Activity.onCreate()
			// (after the setContentView(...) call), or in the Fragment's onActivityCreated method.
			Activity activity = this.Activity;
			ActionBar actionBar = activity.ActionBar;
			actionBar.SetDisplayHomeAsUpEnabled (true);
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState)
		{
			return mPreview;
		}

		public override void OnResume() 
		{
			base.OnResume ();
			
			// Open the default i.e. the first rear facing camera.
			mCamera = Camera.Open (mDefaultCameraId);
			mCameraCurrentlyLocked = mDefaultCameraId;
			mPreview.SetCamera (mCamera);
		}


		public override void OnPause () 
		{
			base.OnPause ();
			
			// Because the Camera object is a shared resource, it's very
			// important to release it when the activity is paused.
			if (mCamera != null) {
				mPreview.SetCamera (null);
				mCamera.Release ();
				mCamera = null;
			}
		}

		public override void OnCreateOptionsMenu (IMenu menu, MenuInflater inflater) 
		{
			if (mNumberOfCameras > 1) {
				// Inflate our menu which can gather user input for switching camera
				inflater.Inflate (Resource.Menu.camera_menu, menu);
			} else {
				base.OnCreateOptionsMenu (menu, inflater);
			}
		}

		public override bool OnOptionsItemSelected (IMenuItem item) {
			// Handle item selection
			switch (item.ItemId) {
			case Resource.Id.switch_cam:
				// Release this camera -> mCameraCurrentlyLocked
				if (mCamera != null) {
					mCamera.StopPreview ();
					mPreview.SetCamera (null);
					mCamera.Release ();
					mCamera = null;
				}
				
				// Acquire the next camera and request Preview to reconfigure
				// parameters.
				mCamera = Camera.Open ((mCameraCurrentlyLocked + 1) % mNumberOfCameras);
				mCameraCurrentlyLocked = (mCameraCurrentlyLocked + 1)
					% mNumberOfCameras;
				mPreview.SwitchCamera (mCamera);
				
				// Start the preview
				mCamera.StartPreview();
				return true;
			case Android.Resource.Id.Home:
				Intent intent = new Intent (this.Activity, typeof (MainActivity));
				intent.AddFlags (ActivityFlags.ClearTop | ActivityFlags.SingleTop);
				StartActivity (intent);
				goto default;
			default:
				return base.OnOptionsItemSelected (item);
			}
		}
	}

	// ----------------------------------------------------------------------
	
	/**
	* A simple wrapper around a Camera and a SurfaceView that renders a centered
	* preview of the Camera to the surface. We need to center the SurfaceView
	* because not all devices have cameras that support preview sizes at the same
	* aspect ratio as the device's display.
	*/
	class Preview : ViewGroup, ISurfaceHolderCallback 
	{
		private const string TAG = "Preview";
			
		SurfaceView mSurfaceView;
		ISurfaceHolder mHolder;
		Camera.Size mPreviewSize;
		IList<Camera.Size> mSupportedPreviewSizes;
		Camera mCamera;
		
		internal Preview (Context context) 
			: base (context)
		{
			mSurfaceView = new SurfaceView (context);
			AddView (mSurfaceView);
			
			// Install a SurfaceHolder.Callback so we get notified when the
			// underlying surface is created and destroyed.
			mHolder = mSurfaceView.Holder;
			mHolder.AddCallback (this);
			mHolder.SetType (SurfaceType.PushBuffers);
		}
			
		public void SetCamera (Camera camera)
		{
			mCamera = camera;
			if (mCamera != null) {
				mSupportedPreviewSizes = mCamera.GetParameters ()
				.SupportedPreviewSizes;
				RequestLayout ();
			}
		}
			
		public void SwitchCamera (Camera camera)
		{
			SetCamera (camera);
			try {
				camera.SetPreviewDisplay (mHolder);
			} catch (IOException exception) {
				Log.Error (TAG, "IOException caused by setPreviewDisplay()", exception);
			}
			Camera.Parameters parameters = camera.GetParameters ();
			parameters.SetPreviewSize (mPreviewSize.Width, mPreviewSize.Height);
			RequestLayout();
			
			camera.SetParameters (parameters);
		}
			
		protected override void OnMeasure (int widthMeasureSpec, int heightMeasureSpec) 
		{
			// We purposely disregard child measurements because act as a
			// wrapper to a SurfaceView that centers the camera preview instead
			// of stretching it.
			int width = ResolveSize (SuggestedMinimumWidth, widthMeasureSpec);
			int height = ResolveSize (SuggestedMinimumHeight, heightMeasureSpec);
			SetMeasuredDimension (width, height);
			
			if (mSupportedPreviewSizes != null) {
				mPreviewSize = GetOptimalPreviewSize (mSupportedPreviewSizes, width, height);
			}
		}
			
		protected override void OnLayout (bool changed, int l, int t, int r, int b) 
		{
			if (changed && ChildCount > 0) {
			View child = GetChildAt (0);
			
			int width = r - l;
			int height = b - t;
			
			int previewWidth = width;
			int previewHeight = height;
			if (mPreviewSize != null) {
				previewWidth = mPreviewSize.Width;
				previewHeight = mPreviewSize.Height;
			}
			
			// Center the child SurfaceView within the parent.
				if (width * previewHeight > height * previewWidth) {
				int scaledChildWidth = previewWidth * height / previewHeight;
				child.Layout ((width - scaledChildWidth) / 2, 0,
					(width + scaledChildWidth) / 2, height);
			} else {
				int scaledChildHeight = previewHeight * width / previewWidth;
				child.Layout (0, (height - scaledChildHeight) / 2, width,
					(height + scaledChildHeight) / 2);
			}
		}
	}

	    public void SurfaceCreated(ISurfaceHolder holder)
	    {
            // The Surface has been created, acquire the camera and tell it where
            // to draw.
            try
            {
                if (mCamera != null)
                {
                    mCamera.SetPreviewDisplay(holder);
                }
            }
            catch (IOException exception)
            {
                Log.Error(TAG, "IOException caused by setPreviewDisplay()", exception);
            }
	        
	    }

	    public void SurfaceDestroyed (ISurfaceHolder holder) {
			// Surface will be destroyed when we return, so stop the preview.
			if (mCamera != null) {
				mCamera.StopPreview ();
			}
		}
			
		private Camera.Size GetOptimalPreviewSize (IList<Camera.Size> sizes, int w, int h) 
		{
			double ASPECT_TOLERANCE = 0.1;
			double targetRatio = (double) w / h;
			if (sizes == null)
				return null;
			
			Camera.Size optimalSize = null;
			double minDiff = double.MaxValue;
			
			int targetHeight = h;
			
			// Try to find an size match aspect ratio and size
			foreach (Camera.Size size in sizes) {
				double ratio = (double) size.Width / size.Height;
				if (Math.Abs (ratio - targetRatio) > ASPECT_TOLERANCE)
					continue;
				if (Math.Abs (size.Height - targetHeight) < minDiff) {
					optimalSize = size;
					minDiff = Math.Abs (size.Height - targetHeight);
				}
			}
			
			// Cannot find the one match the aspect ratio, ignore the requirement
			if (optimalSize == null) {
				minDiff = double.MaxValue;
				foreach (Camera.Size size in sizes) {
					if (Math.Abs (size.Height - targetHeight) < minDiff) {
						optimalSize = size;
						minDiff = Math.Abs (size.Height - targetHeight);
					}
				}
			}
			return optimalSize;
		}
			
		public void SurfaceChanged (ISurfaceHolder holder, Android.Graphics.Format format, int w, int h)
		{
			// Now that the size is known, set up the camera parameters and begin
			// the preview.
			Camera.Parameters parameters = mCamera.GetParameters ();
			parameters.SetPreviewSize (mPreviewSize.Width, mPreviewSize.Height);
			RequestLayout();
			
			mCamera.SetParameters (parameters);
			mCamera.StartPreview ();
		}
	}
}
