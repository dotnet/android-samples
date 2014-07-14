using System;
using Android.Hardware.Camera2;
using Android.Widget;

namespace Camera2VideoSample
{
	public class MyCameraStateListener : CameraDevice.StateListener
	{
		Camera2VideoFragment fragment;
		public MyCameraStateListener(Camera2VideoFragment frag)
		{
			fragment = frag;
		}
		public override void OnOpened (CameraDevice camera)
		{
			fragment.camera_device = camera;
			fragment.startPreview ();
			fragment.opening_camera = false;
			if (null != fragment.texture_view) {
				fragment.configureTransform (fragment.texture_view.Width, fragment.texture_view.Height);
			}
		}

		public override void OnDisconnected (CameraDevice camera)
		{
			camera.Close ();
			fragment.camera_device = null;
			fragment.opening_camera = false;
		}

		public override void OnError (CameraDevice camera, CameraErrorType error)
		{
			camera.Close ();
			fragment.camera_device = null;
			if (null != fragment.Activity) {
				fragment.Activity.Finish ();
			}
			fragment.opening_camera = false;
		}


	}
}

