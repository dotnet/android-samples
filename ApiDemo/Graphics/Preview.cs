using System;
using System.Collections.Generic;
using Android.Content;
using Android.Hardware;
using Android.Views;

namespace MonoDroid.ApiDemo
{
	class Preview : SurfaceView, ISurfaceHolderCallback
	{
		ISurfaceHolder surface_holder;
		Camera camera;

		public Preview (Context context)
			: base (context)
		{
			// Install a SurfaceHolder.Callback so we get notified when the
			// underlying surface is created and destroyed.
			surface_holder = Holder;
			surface_holder.AddCallback (this);
			surface_holder.SetType (SurfaceType.PushBuffers);
		}

		public void SurfaceCreated (ISurfaceHolder holder)
		{
			// The Surface has been created, acquire the camera and tell it where
			// to draw.
			camera = Camera.Open ();

			try {
				camera.SetPreviewDisplay (holder);
			} catch (Exception) {
				camera.Release ();
				camera = null;
				// TODO: add more exception handling logic here
			}
		}

		public void SurfaceDestroyed (ISurfaceHolder holder)
		{
			// Surface will be destroyed when we return, so stop the preview.
			// Because the CameraDevice object is not a shared resource, it's very
			// important to release it when the activity is paused.
			camera.StopPreview ();
			camera.Release ();
			camera = null;
		}

		private Camera.Size GetOptimalPreviewSize (IList<Camera.Size> sizes, int w, int h)
		{
			const double ASPECT_TOLERANCE = 0.05;
			double targetRatio = (double)w / h;

			if (sizes == null)
				return null;

			Camera.Size optimalSize = null;
			double minDiff = Double.MaxValue;
			
			int targetHeight = h;

			// Try to find an size match aspect ratio and size
			for (int i = 0; i < sizes.Count; i++) {
				Camera.Size size = sizes [i];
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
				for (int i = 0; i < sizes.Count; i++) {
					Camera.Size size = sizes [i];

					if (Math.Abs (size.Height - targetHeight) < minDiff) {
						optimalSize = size;
						minDiff = Math.Abs (size.Height - targetHeight);
					}
				}
			}

			return optimalSize;
		}

		public void SurfaceChanged (ISurfaceHolder holder, int format, int w, int h)
		{
			// Now that the size is known, set up the camera parameters and begin
			// the preview.
			Camera.Parameters parameters = camera.GetParameters ();

			IList<Camera.Size> sizes = parameters.SupportedPreviewSizes;
			Camera.Size optimalSize = GetOptimalPreviewSize (sizes, w, h);

			parameters.SetPreviewSize (optimalSize.Width, optimalSize.Height);

			camera.SetParameters (parameters);
			camera.StartPreview ();
		}
	}
}