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
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Hardware;
using Android.OS;
using Android.Views;
using System.Drawing;
using Android.Util;

namespace MonoDroid.ApiDemo
{
	[Activity (Label = "Graphics/Camera Preview",  ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class CameraPreview : Activity
	{
		Preview mPreview;
		Camera mCamera;
		int numberOfCameras;
		int cameraCurrentlyLocked;

		// The first rear facing camera
		int defaultCameraId;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Hide the window title and go fullscreen.
			RequestWindowFeature (WindowFeatures.NoTitle);
			Window.AddFlags (WindowManagerFlags.Fullscreen);

			// Create our Preview view and set it as the content of our activity.
			mPreview = new Preview (this);
			SetContentView (mPreview);

			// Find the total number of cameras available
			numberOfCameras = Camera.NumberOfCameras;

			// Find the ID of the default camera
			Camera.CameraInfo cameraInfo = new Camera.CameraInfo ();
			for (int i = 0; i < numberOfCameras; i++) {
				Camera.GetCameraInfo (i, cameraInfo);
				if (cameraInfo.Facing == CameraFacing.Back) {
					defaultCameraId = i;
				}
			}
		}

		protected override void OnResume ()
		{
			base.OnResume ();

			// Open the default i.e. the first rear facing camera.
			mCamera = Camera.Open ();
			cameraCurrentlyLocked = defaultCameraId;
			mPreview.PreviewCamera = mCamera;
		}

		protected override void OnPause ()
		{
			base.OnPause ();

			// Because the Camera object is a shared resource, it's very
			// important to release it when the activity is paused.
			if (mCamera != null) {
				mPreview.PreviewCamera = null;
				mCamera.Release ();
				mCamera = null;
			}
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			// Inflate our menu which can gather user input for switching camera
			MenuInflater.Inflate (Resource.Menu.camera_menu, menu);
			return true;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			// Handle item selection
			switch (item.ItemId) {
			case Resource.Id.switch_cam:
				// check for availability of multiple cameras
				if (numberOfCameras == 1) {
					AlertDialog.Builder builder = new AlertDialog.Builder (this);
					builder.SetMessage (GetString (Resource.String.camera_alert))
						.SetNeutralButton ("Close", (Android.Content.IDialogInterfaceOnClickListener)null);
					AlertDialog alert = builder.Create ();
					alert.Show ();
					return true;
				}

				// OK, we have multiple cameras.
				// Release this camera -> cameraCurrentlyLocked
				if (mCamera != null) {
					mCamera.StopPreview ();
					mPreview.PreviewCamera = null;
					mCamera.Release ();
					mCamera = null;
				}

				// Acquire the next camera and request Preview to reconfigure
				// parameters.
				mCamera = Camera.Open ((cameraCurrentlyLocked + 1) % numberOfCameras);
				cameraCurrentlyLocked = (cameraCurrentlyLocked + 1) % numberOfCameras;
				mPreview.SwitchCamera (mCamera);

				// Start the preview
				mCamera.StartPreview ();
				return true;

			default:
				return base.OnOptionsItemSelected (item);
			}
		}
	}

	// ----------------------------------------------------------------------

	/**
 	* A simple wrapper around a Camera and a SurfaceView that renders a centered preview of the Camera
 	* to the surface. We need to center the SurfaceView because not all devices have cameras that
 	* support preview sizes at the same aspect ratio as the device's display.
 	*/
	//FIXME	
	class Preview : ViewGroup,  ISurfaceHolderCallback
	{
		string TAG = "Preview";

		SurfaceView mSurfaceView;
		ISurfaceHolder mHolder;
		Camera.Size mPreviewSize;
		IList<Camera.Size> mSupportedPreviewSizes;
		Camera _camera;

		public Camera PreviewCamera {
			get { return _camera; } 
			set {
				_camera = value;
				if (_camera != null) {
					mSupportedPreviewSizes = PreviewCamera.GetParameters().SupportedPreviewSizes;
						RequestLayout ();
				}
			}
		}

		public Preview (Context context) : base (context)
		{
			mSurfaceView = new SurfaceView (context);
			AddView (mSurfaceView);

			// Install a SurfaceHolder.Callback so we get notified when the
			// underlying surface is created and destroyed.
			mHolder = mSurfaceView.Holder;
			mHolder.AddCallback (this);
			mHolder.SetType (SurfaceType.PushBuffers);
		}

		public void SwitchCamera (Camera camera)
		{
			PreviewCamera = camera;

			try {
				camera.SetPreviewDisplay (mHolder);
			} catch (Java.IO.IOException exception) {
					Log.Error (TAG, "IOException caused by setPreviewDisplay()", exception);
			}

			Camera.Parameters parameters = camera.GetParameters ();
			parameters.SetPreviewSize (mPreviewSize.Width, mPreviewSize.Height);
			RequestLayout ();

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
					child.Layout (0, (height - scaledChildHeight) / 2,
					             width, (height + scaledChildHeight) / 2);
				}
			}
		}

		public void SurfaceCreated (ISurfaceHolder holder)
		{
			// The Surface has been created, acquire the camera and tell it where
			// to draw.
			try {
				if (PreviewCamera != null) {
					PreviewCamera.SetPreviewDisplay (holder);
				}
			} catch (Java.IO.IOException exception) {
				Log.Error (TAG, "IOException caused by setPreviewDisplay()", exception);
			}
		}

		public void SurfaceDestroyed (ISurfaceHolder holder)
		{
			// Surface will be destroyed when we return, so stop the preview.
			if (PreviewCamera != null) {
				PreviewCamera.StopPreview ();
			}
		}

		private Camera.Size GetOptimalPreviewSize (IList<Camera.Size> sizes, int w, int h)
		{
			const double ASPECT_TOLERANCE = 0.1;
			double targetRatio = (double)w / h;

			if (sizes == null)
				return null;

			Camera.Size optimalSize = null;
			double minDiff = Double.MaxValue;
			
			int targetHeight = h;

			// Try to find an size match aspect ratio and size
			foreach (Camera.Size size in sizes) {
				double ratio = (double)size.Width / size.Height;

				if (Math.Abs (ratio - targetRatio) > ASPECT_TOLERANCE)
					continue;

				if (Math.Abs (size.Height - targetHeight) < minDiff) {
					optimalSize = size;
					minDiff = Math.Abs (size.Height - targetHeight);
				}
			}

			// Cannot find the one match the aspect ratio, ignore the requirement
			if (optimalSize == null) {
				minDiff = Double.MaxValue;
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
			Camera.Parameters parameters = PreviewCamera.GetParameters ();
			parameters.SetPreviewSize (mPreviewSize.Width, mPreviewSize.Height);
			RequestLayout ();

			PreviewCamera.SetParameters (parameters);
			PreviewCamera.StartPreview ();
		}
	}
}
