using System;
using Android.Hardware.Camera2;
using Android.Widget;

namespace Camera2VideoSample
{
	public class MyCameraStateCallback : CameraDevice.StateCallback
	{
		Camera2VideoFragment fragment;
		public MyCameraStateCallback(Camera2VideoFragment frag)
		{
			fragment = frag;
		}
		public override void OnOpened (CameraDevice camera)
		{
			fragment.cameraDevice = camera;
			fragment.startPreview ();
			fragment.cameraOpenCloseLock.Release ();
			if (null != fragment.textureView) 
				fragment.configureTransform (fragment.textureView.Width, fragment.textureView.Height);
		}

		public override void OnDisconnected (CameraDevice camera)
		{
			fragment.cameraOpenCloseLock.Release ();
			camera.Close ();
			fragment.cameraDevice = null;
		}

		public override void OnError (CameraDevice camera, CameraError error)
		{
			fragment.cameraOpenCloseLock.Release ();
			camera.Close ();
			fragment.cameraDevice = null;
			if (null != fragment.Activity) 
				fragment.Activity.Finish ();
		}


	}
}

