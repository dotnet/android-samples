
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
 	* Displays a CameraPreview of the first Camera.
 	* An error message is displayed if the Camera is not available.
 	* 
 	* This Fragment is only used to illustrate that access to the Camera API has been granted (or
 	* denied) as part of the runtime permissions model. It is not relevant for the use of the
 	* permissions API.
 	* 
 	* Implementation is based directly on the documentation at
 	* http://developer.android.com/guide/topics/media/camera.html
 	*/
	public class CameraPreviewFragment : Fragment
	{
		static readonly string TAG = "CameraPreview";

		/**
     	* Id of the camera to access. 0 is the first camera.
     	*/
		static readonly int CAMERA_ID = 0;

		CameraPreview cameraPreview;
		Camera camera;

		public static CameraPreviewFragment NewInstance ()
		{
			return new CameraPreviewFragment ();
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Open an instance of the first camera and retrieve its info.
			camera = GetCameraInstance (CAMERA_ID);
			Camera.CameraInfo cameraInfo = null;

			if (camera != null) {
				// Get camera info only if the camera is available
				cameraInfo = new Camera.CameraInfo ();
				Camera.GetCameraInfo (CAMERA_ID, cameraInfo);
			}

			if (camera == null || cameraInfo == null) {
				Toast.MakeText (Activity, "Camera is not available.", ToastLength.Short).Show ();
				return inflater.Inflate (Resource.Layout.fragment_camera_unavailable, null);
			}

			View root = inflater.Inflate (Resource.Layout.fragment_camera, null);

			// Get the rotation of the screen to adjust the preview image accordingly.
			SurfaceOrientation displayRotation = Activity.WindowManager.DefaultDisplay.Rotation;

			// Create the Preview view and set it as the content of this Activity.
			cameraPreview = new CameraPreview (Activity, camera, cameraInfo, displayRotation);
			var preview =  root.FindViewById <FrameLayout> (Resource.Id.camera_preview);
			preview.AddView (cameraPreview);

			return root;
		}

		public override void OnPause ()
		{
			base.OnPause ();
			// Stop camera access
			ReleaseCamera ();
		}
			
		/** A safe way to get an instance of the Camera object. */
		public static Camera GetCameraInstance (int cameraId)
		{
			Camera c = null;

			try {
				c = Camera.Open (cameraId);	// attempt to get a Camera instance
			} catch (Exception e) {
				// Camera is not available (in use or does not exist)
				CommonSampleLibrary.Log.Debug (TAG, string.Format ("Camera {0} is not available: {1}", cameraId,  e.Message));
			}

			return c;	// returns null if camera is unavailable
		}

		void ReleaseCamera ()
		{
			if (camera != null) {
				camera.Release ();	// release the camera for other applications
				camera = null;
			}
		}
	}
}

