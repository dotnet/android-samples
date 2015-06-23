
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Hardware;

namespace RuntimePermissions
{
	/**
 	* Camera preview that displays a {@link Camera}.
 	*
 	* Handles basic lifecycle methods to display and stop the preview.
 	* <p>
 	* Implementation is based directly on the documentation at
 	* http://developer.android.com/guide/topics/media/camera.html
 	*/
	public class CameraPreview : SurfaceView, ISurfaceHolderCallback
	{
		static readonly string TAG = "CameraPreview";
		ISurfaceHolder holder;
		Camera camera;
		Camera.CameraInfo cameraInfo;
		SurfaceOrientation displayOrientation;

		public CameraPreview (Context context, Camera camera, Camera.CameraInfo cameraInfo, 
			SurfaceOrientation displayOrientation) :
			base (context)
		{
			// Do not initialize if no camera has been set
			if (camera == null || cameraInfo == null)
				return;
			
			this.camera = camera;
			this.cameraInfo = cameraInfo;
			this.displayOrientation = displayOrientation;

			// Install a SurfaceHolder.Callback so we get notified when the
			// underlying surface is created and destroyed.
			holder = Holder;
			holder.AddCallback (this);
		}

		public void SurfaceCreated (ISurfaceHolder holder)
		{
			// The Surface has been created, now tell the camera where to draw the preview.
			try {
				camera.SetPreviewDisplay (holder);
				camera.StartPreview ();
				CommonSampleLibrary.Log.Debug (TAG, "Camera preview started.");
			} catch (Exception e) {
				Log.Debug (TAG, "Error setting camera preview: " + e.Message);
			}
		}

		public void SurfaceDestroyed (ISurfaceHolder holder)
		{
			// empty. Take care of releasing the Camera preview in your activity.
		}

		public void SurfaceChanged (ISurfaceHolder holder, Android.Graphics.Format format, int w, int h)
		{
			// If your preview can change or rotate, take care of those events here.
			// Make sure to stop the preview before resizing or reformatting it.

			if (holder.Surface == null) {
				// preview surface does not exist
				CommonSampleLibrary.Log.Debug (TAG, "Preview surface does not exist");
				return;
			}

			// stop preview before making changes
			try {
				camera.StopPreview ();
				CommonSampleLibrary.Log.Debug (TAG, "Preview stopped.");
			} catch (Exception e) {
				// ignore: tried to stop a non-existent preview
				CommonSampleLibrary.Log.Debug(TAG, "Error starting camera preview: " + e.Message);
			}

			int orientation = CalculatePreviewOrientation (cameraInfo, displayOrientation);
			camera.SetDisplayOrientation (orientation);

			try {
				camera.SetPreviewDisplay (holder);
				camera.StartPreview ();
				CommonSampleLibrary.Log.Debug (TAG, "Camera preview started.");
			} catch (Exception e) {
				CommonSampleLibrary.Log.Debug (TAG, "Error starting camera preview: " + e.Message);
			}
		}

		/**
     	* Calculate the correct orientation for a {@link Camera} preview that is displayed on screen.
     	*
     	* Implementation is based on the sample code provided in
     	* {@link Camera#setDisplayOrientation(int)}.
     	*/
		public static int CalculatePreviewOrientation (Camera.CameraInfo info, SurfaceOrientation rotation)
		{
			int degrees = 0;

			switch (rotation) {
			case SurfaceOrientation.Rotation0:
				degrees = 0;
				break;
			case SurfaceOrientation.Rotation90:
				degrees = 90;
				break;
			case SurfaceOrientation.Rotation180:
				degrees = 180;
				break;
			case SurfaceOrientation.Rotation270:
				degrees = 270;
				break;
			}

			int result;
			if (info.Facing == CameraFacing.Front) {
				result = (info.Orientation + degrees) % 360;
				result = (360 - result) % 360;  // compensate the mirror
			} else {  // back-facing
				result = (info.Orientation - degrees + 360) % 360;
			}

			return result;
		}
	}
}

