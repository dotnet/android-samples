using System;
using Android.Hardware.Camera2;
using Android.Widget;

namespace Camera2VideoSample
{
	public class PreviewCaptureStateListener: CameraCaptureSession.StateListener
	{
		Camera2VideoFragment fragment;
		public PreviewCaptureStateListener(Camera2VideoFragment frag)
		{
			fragment = frag;
		}
		public override void OnConfigured (CameraCaptureSession session)
		{
			fragment.preview_session = session;
			fragment.updatePreview ();

		}

		public override void OnConfigureFailed (CameraCaptureSession session)
		{
			if (null != fragment.Activity) 
				Toast.MakeText (fragment.Activity, "Failed", ToastLength.Short).Show ();
		}
	}
}

