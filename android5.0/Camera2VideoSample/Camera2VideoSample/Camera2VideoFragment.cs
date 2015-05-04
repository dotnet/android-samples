
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Hardware.Camera2;
using Android.Hardware.Camera2.Params;
using Android.Media;
using NUnit.Framework;

using Java.Lang;
using Java.Util;
using Java.IO;
using Java.Util.Concurrent;


namespace Camera2VideoSample
{
	public class Camera2VideoFragment : Fragment, View.IOnClickListener
	{
		private const string TAG = "Camera2VideoFragment";
		private SparseIntArray ORIENTATIONS = new SparseIntArray ();

		// Button to record video
		private Button buttonVideo;

		// AutoFitTextureView for camera preview
		public AutoFitTextureView textureView;

		public CameraDevice cameraDevice;
		public CameraCaptureSession previewSession;
		public MediaRecorder mediaRecorder;

		private bool isRecordingVideo;
		public Semaphore cameraOpenCloseLock = new Semaphore (1);

		// Called when the CameraDevice changes state
		private MyCameraStateCallback stateListener;
		// Handles several lifecycle events of a TextureView
		private MySurfaceTextureListener surfaceTextureListener;

		public CaptureRequest.Builder builder;
		private CaptureRequest.Builder previewBuilder;

		private Size videoSize;	
		private Size previewSize;


		private HandlerThread backgroundThread;
		private Handler backgroundHandler;


		public Camera2VideoFragment()
		{
			ORIENTATIONS.Append ((int)SurfaceOrientation.Rotation0, 90);
			ORIENTATIONS.Append ((int)SurfaceOrientation.Rotation90, 0);
			ORIENTATIONS.Append ((int)SurfaceOrientation.Rotation180, 270);
			ORIENTATIONS.Append ((int)SurfaceOrientation.Rotation270, 180);
			surfaceTextureListener = new MySurfaceTextureListener(this);
			stateListener = new MyCameraStateCallback (this);
		}
		public static Camera2VideoFragment newInstance(){
			var fragment = new Camera2VideoFragment();
			fragment.RetainInstance = true;
			return fragment;
		}

		private Size ChooseVideoSize(Size[] choices)
		{
			foreach (Size size in choices) {
				if (size.Width == size.Height * 4 / 3 && size.Width <= 1000)
					return size;
			}
			Log.Error (TAG, "Couldn't find any suitable video size");
			return choices [choices.Length - 1];
		}

		private Size ChooseOptimalSize(Size[] choices, int width, int height, Size aspectRatio)
		{
			var bigEnough = new List<Size> ();
			int w = aspectRatio.Width;
			int h = aspectRatio.Height;
			foreach (Size option in choices) {
				if (option.Height == option.Width * h / w &&
					option.Width >= width && option.Height >= height)
					bigEnough.Add (option);
			}

			if (bigEnough.Count > 0)
				return (Size)Collections.Min (bigEnough, new CompareSizesByArea ());
			else {
				Log.Error (TAG, "Couldn't find any suitable preview size");
				return choices [0];
			}
		}
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			;
			return inflater.Inflate (Resource.Layout.fragment_camera2_video, container, false);
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			textureView = (AutoFitTextureView)view.FindViewById (Resource.Id.texture);
			buttonVideo = (Button)view.FindViewById (Resource.Id.video);
			buttonVideo.SetOnClickListener (this);
			view.FindViewById (Resource.Id.info).SetOnClickListener (this);

		}

		public override void OnResume ()
		{
			base.OnResume ();
			StartBackgroundThread ();
			if (textureView.IsAvailable)
				openCamera (textureView.Width, textureView.Height);
			else
				textureView.SurfaceTextureListener = surfaceTextureListener;

		}

		public override void OnPause ()
		{
			CloseCamera ();
			StopBackgroundThread ();
			base.OnPause ();
		}

		private void StartBackgroundThread()
		{
			backgroundThread = new HandlerThread ("CameraBackground");
			backgroundThread.Start ();
			backgroundHandler = new Handler (backgroundThread.Looper);
		}

		private void StopBackgroundThread()
		{
			backgroundThread.QuitSafely ();
			try {
				backgroundThread.Join();
				backgroundThread = null;
				backgroundHandler = null;
			} catch(InterruptedException e) {
				e.PrintStackTrace ();
			}
		}

		public void OnClick(View view)
		{
			switch (view.Id) {
			case Resource.Id.video:
				{
					if (isRecordingVideo) {
						stopRecordingVideo ();
					} else {
						StartRecordingVideo ();
					}
					break;
				}

			case Resource.Id.info:
				{
					if (null != Activity) {
						new AlertDialog.Builder (Activity)
							.SetMessage (Resource.String.intro_message)
							.SetPositiveButton (Android.Resource.String.Ok, (Android.Content.IDialogInterfaceOnClickListener)null)
							.Show ();
					}
					break;
				}
			}
		}

		//Tries to open a CameraDevice
		public void openCamera(int width, int height)
		{
			if (null == Activity || Activity.IsFinishing) 
				return;

			CameraManager manager = (CameraManager)Activity.GetSystemService (Context.CameraService);
			try {
				if(!cameraOpenCloseLock.TryAcquire(2500,TimeUnit.Milliseconds))
					throw new RuntimeException("Time out waiting to lock camera opening.");
				string cameraId = manager.GetCameraIdList()[0];
				CameraCharacteristics characteristics = manager.GetCameraCharacteristics(cameraId);
				StreamConfigurationMap map = (StreamConfigurationMap)characteristics.Get(CameraCharacteristics.ScalerStreamConfigurationMap);
				videoSize = ChooseVideoSize(map.GetOutputSizes(Class.FromType(typeof(MediaRecorder))));
				previewSize = ChooseOptimalSize(map.GetOutputSizes(Class.FromType(typeof(MediaRecorder))),width,height,videoSize);
				int orientation = (int)Resources.Configuration.Orientation;
				if(orientation == (int)Android.Content.Res.Orientation.Landscape){
					textureView.SetAspectRatio(previewSize.Width,previewSize.Height);
				} else {
					textureView.SetAspectRatio(previewSize.Height,previewSize.Width);
				}
				configureTransform(width,height);
				mediaRecorder = new MediaRecorder();
				manager.OpenCamera(cameraId,stateListener,null);

			} catch (CameraAccessException) {
				Toast.MakeText (Activity, "Cannot access the camera.", ToastLength.Short).Show ();
				Activity.Finish ();
			} catch (NullPointerException) {
				var dialog = new ErrorDialog ();
				dialog.Show (FragmentManager, "dialog");
			} catch (InterruptedException) {
				throw new RuntimeException ("Interrupted while trying to lock camera opening.");
			}
		}

		//Start the camera preview
		public void startPreview()
		{
			if (null == cameraDevice || !textureView.IsAvailable || null == previewSize) 
				return;

			try {
				SetUpMediaRecorder();
				SurfaceTexture texture = textureView.SurfaceTexture;
				//Assert.IsNotNull(texture);
				texture.SetDefaultBufferSize(previewSize.Width,previewSize.Height);
				previewBuilder = cameraDevice.CreateCaptureRequest(CameraTemplate.Record);
				var surfaces = new List<Surface>();
				var previewSurface = new Surface(texture);
				surfaces.Add(previewSurface);
				previewBuilder.AddTarget(previewSurface);

				var recorderSurface = mediaRecorder.Surface;
				surfaces.Add(recorderSurface);
				previewBuilder.AddTarget(recorderSurface);

				cameraDevice.CreateCaptureSession(surfaces, new PreviewCaptureStateCallback(this),backgroundHandler);

			} catch(CameraAccessException e) {
				e.PrintStackTrace ();
			} catch(IOException e) {
				e.PrintStackTrace ();
			}
		}

		private void CloseCamera()
		{
			try {
				cameraOpenCloseLock.Acquire();
				if(null != cameraDevice) {
					cameraDevice.Close();
					cameraDevice = null;
				}
				if(null != mediaRecorder) {
					mediaRecorder.Release();
					mediaRecorder = null;
				}
			} catch (InterruptedException e) {
				throw new RuntimeException ("Interrupted while trying to lock camera closing.");
			} finally {
				cameraOpenCloseLock.Release ();
			}
		}

		//Update the preview
		public void updatePreview() 
		{
			if (null == cameraDevice) 
				return;

			try {
				setUpCaptureRequestBuilder(previewBuilder);
				HandlerThread thread = new HandlerThread("CameraPreview");
				thread.Start();
				previewSession.SetRepeatingRequest(previewBuilder.Build(),null,backgroundHandler);
			} catch(CameraAccessException e) {
				e.PrintStackTrace ();
			}
		}

		private void setUpCaptureRequestBuilder(CaptureRequest.Builder builder)
		{
			builder.Set (CaptureRequest.ControlMode,new Java.Lang.Integer((int) ControlMode.Auto));

		}

		//Configures the neccesary matrix transformation to apply to the textureView
		public void configureTransform(int viewWidth, int viewHeight) 
		{
			if (null == Activity || null == previewSize || null == textureView) 
				return;

			int rotation = (int)Activity.WindowManager.DefaultDisplay.Rotation;
			var matrix = new Matrix ();
			var viewRect = new RectF (0, 0, viewWidth, viewHeight);
			var bufferRect = new RectF (0, 0, previewSize.Height, previewSize.Width);
			float centerX = viewRect.CenterX();
			float centerY = viewRect.CenterY();
			if ((int)SurfaceOrientation.Rotation90 == rotation || (int)SurfaceOrientation.Rotation270 == rotation) { 
				bufferRect.Offset ((centerX - bufferRect.CenterX()), (centerY - bufferRect.CenterY()));
				matrix.SetRectToRect (viewRect, bufferRect, Matrix.ScaleToFit.Fill);
				float scale = System.Math.Max (
					(float)viewHeight / previewSize.Height,
					(float)viewHeight / previewSize.Width);
				matrix.PostScale (scale, scale, centerX, centerY);
				matrix.PostRotate (90 * (rotation - 2), centerX, centerY);
			}
			textureView.SetTransform (matrix);
		}

		private void SetUpMediaRecorder() {
			if (null == Activity)
				return;
			mediaRecorder.SetAudioSource (AudioSource.Mic);
			mediaRecorder.SetVideoSource (VideoSource.Surface);
			mediaRecorder.SetOutputFormat (OutputFormat.Mpeg4);
			mediaRecorder.SetOutputFile (GetVideoFile(Activity).AbsolutePath);
			mediaRecorder.SetVideoEncodingBitRate (10000000);
			mediaRecorder.SetVideoFrameRate (30);
			mediaRecorder.SetVideoSize (videoSize.Width,videoSize.Height);
			mediaRecorder.SetVideoEncoder (VideoEncoder.H264);
			mediaRecorder.SetAudioEncoder (AudioEncoder.Aac);
			int rotation = (int)Activity.WindowManager.DefaultDisplay.Rotation;
			int orientation = ORIENTATIONS.Get (rotation);
			mediaRecorder.SetOrientationHint (orientation);
			mediaRecorder.Prepare ();
		}

		private File GetVideoFile(Context context)
		{
			return new File (context.GetExternalFilesDir (null), "video.mp4");
		}

		private void StartRecordingVideo()
		{
			try {
				//UI
				buttonVideo.SetText (Resource.String.stop);
				isRecordingVideo = true;

				//Start recording
				mediaRecorder.Start();
			}
			catch (IllegalStateException e) {
				e.PrintStackTrace ();
			}
		}

		public void stopRecordingVideo() 
		{
			//UI
			isRecordingVideo = false;
			buttonVideo.SetText (Resource.String.record);

			if (null != Activity) {
				Toast.MakeText (Activity, "Video saved: " + GetVideoFile (Activity),
					ToastLength.Short).Show ();
			}

			//Stop recording
			/*
			mediaRecorder.Stop ();
			mediaRecorder.Reset ();
			startPreview ();
			*/

			// Workaround for https://github.com/googlesamples/android-Camera2Video/issues/2
			CloseCamera ();
			openCamera (textureView.Width, textureView.Height);
		}

		public class ErrorDialog : DialogFragment {
			public override Dialog OnCreateDialog (Bundle savedInstanceState)
			{
				var alert = new AlertDialog.Builder (Activity);
				alert.SetMessage ("This device doesn't support Camera2 API.");
				alert.SetPositiveButton (Android.Resource.String.Ok, new MyDialogOnClickListener (this));
				return alert.Show();

			}
		}

		private class MyDialogOnClickListener : Java.Lang.Object,IDialogInterfaceOnClickListener
		{
			ErrorDialog er;
			public MyDialogOnClickListener(ErrorDialog e)
			{
				er = e;
			}
			public void OnClick(IDialogInterface dialogInterface, int i)
			{
				er.Activity.Finish ();
			}
		}

		// Compare two Sizes based on their areas
		private class CompareSizesByArea : Java.Lang.Object,Java.Util.IComparator
		{
			public int Compare(Java.Lang.Object lhs, Java.Lang.Object rhs) {
				// We cast here to ensure the multiplications won't overflow
				if (lhs is Size && rhs is Size) {
					var right = (Size)rhs;
					var left = (Size)lhs;
					return Long.Signum ((long)left.Width * left.Height -
						(long)right.Width * right.Height);
				} else
					return 0;

			}
		}
	}
}

