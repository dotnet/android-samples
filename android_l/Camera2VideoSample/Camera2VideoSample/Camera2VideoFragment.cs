
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


namespace Camera2VideoSample
{
	public class Camera2VideoFragment : Fragment, View.IOnClickListener
	{
		private SparseIntArray ORIENTATIONS = new SparseIntArray ();

		private Button button_video;

		public AutoFitTextureView texture_view;

		public CameraDevice camera_device;

		public CameraCaptureSession preview_session;

		public MediaRecorder media_recorder;
		private bool is_recording_video;
		public bool opening_camera;

		//Called when the CameraDevice changes state
		private MyCameraStateListener state_listener;

		//Handles several lifecycle events of a TextureView
		private MySurfaceTextureListener surface_texture_listener;
		public CaptureRequest.Builder builder;

			
		private Size preview_size;
		private CaptureRequest.Builder preview_builder;


		public Camera2VideoFragment()
		{
			ORIENTATIONS.Append ((int)SurfaceOrientation.Rotation0, 90);
			ORIENTATIONS.Append ((int)SurfaceOrientation.Rotation90, 0);
			ORIENTATIONS.Append ((int)SurfaceOrientation.Rotation180, 270);
			ORIENTATIONS.Append ((int)SurfaceOrientation.Rotation270, 180);
			surface_texture_listener = new MySurfaceTextureListener(this);
			state_listener = new MyCameraStateListener (this);
		}
		public static Camera2VideoFragment newInstance(){
			var fragment = new Camera2VideoFragment();
			fragment.RetainInstance = true;
			return fragment;
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
;
			return inflater.Inflate (Resource.Layout.fragment_camera2_video, container, false);
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			texture_view = (AutoFitTextureView)view.FindViewById (Resource.Id.texture);
			texture_view.SurfaceTextureListener = surface_texture_listener;
			button_video = (Button)view.FindViewById (Resource.Id.video);
			button_video.SetOnClickListener (this);
			view.FindViewById (Resource.Id.info).SetOnClickListener (this);

		}

		public override void OnResume ()
		{
			base.OnResume ();
			openCamera ();
		}

		public override void OnPause ()
		{
			base.OnPause ();
			if (null != camera_device) {
				camera_device.Close ();
				camera_device = null;
			}
		}
			
		public void OnClick(View view)
		{
			switch (view.Id) {
			case Resource.Id.video:
				{
					if (is_recording_video) {
						stopRecordingVideo ();
					} else {
						startRecordingVideo ();
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
		public void openCamera()
		{
			if (null == Activity || Activity.IsFinishing || opening_camera) 
				return;

			opening_camera = true;
			CameraManager manager = (CameraManager)Activity.GetSystemService (Context.CameraService);
			try {
				string camera_id = manager.GetCameraIdList()[0];
				CameraCharacteristics characteristics = manager.GetCameraCharacteristics(camera_id);
				StreamConfigurationMap map = (StreamConfigurationMap)characteristics.Get(CameraCharacteristics.ScalerStreamConfigurationMap);
				preview_size = map.GetOutputSizes(Java.Lang.Class.FromType(typeof(SurfaceTexture)))[0];
				int orientation = (int)Resources.Configuration.Orientation;
				if(orientation == (int)Android.Content.Res.Orientation.Landscape){
					texture_view.SetAspectRatio(preview_size.Width,preview_size.Height);
				} else {
					texture_view.SetAspectRatio(preview_size.Height,preview_size.Width);
				}
				manager.OpenCamera(camera_id,state_listener,null);

			} catch (CameraAccessException) {
				Toast.MakeText (Activity, "Cannot access the camera.", ToastLength.Short).Show ();
				Activity.Finish ();
			} catch (NullPointerException) {
				var dialog = new ErrorDialog ();
				dialog.Show (FragmentManager, "dialog");
			}
		}

		//Start the camera preview
		public void startPreview()
		{
			if (null == camera_device || !texture_view.IsAvailable || null == preview_size) 
				return;

			try {
				SurfaceTexture texture = texture_view.SurfaceTexture;
				//Assert.IsNotNull(texture);
				texture.SetDefaultBufferSize(preview_size.Width,preview_size.Height);
				preview_builder = camera_device.CreateCaptureRequest(CameraTemplate.Preview);
				Surface surface = new Surface(texture);
				var surfaces = new List<Surface>();
				surfaces.Add(surface);
				preview_builder.AddTarget(surface);
				camera_device.CreateCaptureSession(surfaces, new PreviewCaptureStateListener(this),null);


			} catch(CameraAccessException e) {
				e.PrintStackTrace ();
			}
		}

		//Update the preview
		public void updatePreview() 
		{
			if (null == camera_device) 
				return;

			try {
				setUpCaptureRequestBuilder(preview_builder);
				HandlerThread thread = new HandlerThread("CameraPreview");
				thread.Start();
				Handler background_handler = new Handler(thread.Looper);
				preview_session.SetRepeatingRequest(preview_builder.Build(),null,background_handler);
			} catch(CameraAccessException e) {
				e.PrintStackTrace ();
			}
		}

		private void setUpCaptureRequestBuilder(CaptureRequest.Builder builder)
		{
			builder.Set (CaptureRequest.ControlMode,new Java.Lang.Integer((int) ControlMode.Auto));

		}

		//Configures the neccesary matrix transformation to apply to the texture_view
		public void configureTransform(int viewWidth, int viewHeight) 
		{
			if (null == Activity || null == preview_size || null == texture_view) 
				return;

			int rotation = (int)Activity.WindowManager.DefaultDisplay.Rotation;
			var matrix = new Matrix ();
			var view_rect = new RectF (0, 0, viewWidth, viewHeight);
			var buffer_rect = new RectF (0, 0, preview_size.Height, preview_size.Width);
			float center_x = view_rect.CenterX();
			float center_y = view_rect.CenterY();
			if ((int)SurfaceOrientation.Rotation90 == rotation || (int)SurfaceOrientation.Rotation270 == rotation) { 
				buffer_rect.Offset ((center_x - buffer_rect.CenterX()), (center_y - buffer_rect.CenterY()));
				matrix.SetRectToRect (view_rect, buffer_rect, Matrix.ScaleToFit.Fill);
				float scale = System.Math.Max (
					(float)viewHeight / preview_size.Height,
					(float)viewHeight / preview_size.Width);
				matrix.PostScale (scale, scale, center_x, center_y);
				matrix.PostRotate (90 * (rotation - 2), center_x, center_y);
			}
			texture_view.SetTransform (matrix);

		}

		private void startRecordingVideo() {
			if (null == Activity) 
				return;


			media_recorder = new MediaRecorder ();
			File file = GetVideoFile (Activity);
			try {
				//UI
				button_video.SetText (Resource.String.stop);
				is_recording_video = true;

				//Configure the MediaRecorder

				media_recorder.SetAudioSource (AudioSource.Mic);
				media_recorder.SetVideoSource (VideoSource.Surface);
				media_recorder.SetOutputFormat (OutputFormat.Mpeg4);
				media_recorder.SetOutputFile (System.IO.Path.GetFullPath (file.ToString()));
				media_recorder.SetVideoEncodingBitRate (10000000);
				media_recorder.SetVideoFrameRate (30);
				media_recorder.SetVideoSize (720, 480);
				media_recorder.SetVideoEncoder (VideoEncoder.H264);
				media_recorder.SetAudioEncoder (AudioEncoder.Aac);
				int rotation = (int)Activity.WindowManager.DefaultDisplay.Rotation;
				int orientation = ORIENTATIONS.Get (rotation);
				media_recorder.SetOrientationHint (orientation);
				media_recorder.Prepare ();

				Surface surface = media_recorder.Surface;

				//Set up CaptureRequest
				builder = camera_device.CreateCaptureRequest (CameraTemplate.Record);
				builder.AddTarget (surface);
				var preview_surface = new Surface (texture_view.SurfaceTexture);
				builder.AddTarget (preview_surface);
				var surface_list = new List<Surface>();
				surface_list.Add(surface);
				surface_list.Add(preview_surface);
				camera_device.CreateCaptureSession(surface_list,new RecordingCaptureStateListener(this),null);

			} catch (IOException e) {
				e.PrintStackTrace ();
			} catch (CameraAccessException e) {
				e.PrintStackTrace ();
			} catch (IllegalStateException e) {
				e.PrintStackTrace ();
			}

		}
		private File GetVideoFile(Context context) 
		{
			return new File (context.GetExternalFilesDir (null), "video.mp4");
		}

		public void stopRecordingVideo() 
		{
			//UI
			is_recording_video = false;
			button_video.SetText (Resource.String.record);

			//Stop recording
			media_recorder.Stop ();
			media_recorder.Release ();
			media_recorder = null;
			if (null != Activity) {
				Toast.MakeText (Activity, "Video saved: " + GetVideoFile (Activity),
					ToastLength.Short).Show ();
			}
			startPreview ();
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
	}








}

