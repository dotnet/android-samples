using System;
using Android.Hardware.Camera2;
using Android.Widget;

namespace Camera2VideoSample
{
	public class RecordingCaptureStateListener: CameraCaptureSession.StateListener
	{
		Camera2VideoFragment fragment;
		public RecordingCaptureStateListener(Camera2VideoFragment frag)
		{
			fragment = frag;
		}
		public override void OnConfigured (CameraCaptureSession session)
		{
			//Start Recording
			try {
				session.SetRepeatingRequest(fragment.builder.Build(),null,null);
				fragment.media_recorder.Start();
			} catch (CameraAccessException e) {
				e.PrintStackTrace ();
			}

		}

		public override void OnConfigureFailed (CameraCaptureSession session)
		{
			if (null != fragment.Activity) {
				Toast.MakeText (fragment.Activity, "Failed", ToastLength.Short).Show ();
			}
		}
	}
}

