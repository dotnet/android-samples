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
using Java.Util.Concurrent.Atomic;
using Java.Util.Concurrent;
using Android.Hardware.Camera2;
using Android.Media;
using Java.Lang;
using Java.Util;
using Android.Hardware;
using Android.Hardware.Camera2.Params;
using Android.Graphics;
using Java.Nio;
using Java.Text;
using System.IO;

namespace Camera2Raw
{
	public class Camera2RawFragment : Fragment, View.IOnClickListener
	{
		static readonly SparseIntArray ORIENTATIONS = new SparseIntArray ();

		static Camera2RawFragment ()
		{
			ORIENTATIONS.Append ((int)SurfaceOrientation.Rotation0, 0);
			ORIENTATIONS.Append ((int)SurfaceOrientation.Rotation90, 90);
			ORIENTATIONS.Append ((int)SurfaceOrientation.Rotation180, 180);
			ORIENTATIONS.Append ((int)SurfaceOrientation.Rotation270, 270);
		}

		public Camera2RawFragment () : base ()
		{
		}

		/// <summary>
		/// Timeout for the pre-capture sequence.
		/// </summary>
		const long PRECAPTURE_TIMEOUT_MS = 1000;

		/// <summary>
		/// Tolerance when comparing aspect ratios.
		/// </summary>
		const double ASPECT_RATIO_TOLERANCE = 0.005;

		/// <summary>
		/// Tag for the {@link Log}.
		/// </summary>
		const string TAG = "Camera2RawFragment";

		/// <summary>
		/// Camera state: Device is closed.
		/// </summary>
		const int STATE_CLOSED = 0;

		/// <summary>
		/// Camera state: Device is opened, but is not capturing.
		/// </summary>
		const int STATE_OPENED = 1;

		/// <summary>
		/// Camera state: Showing camera preview.
		/// </summary>
		const int STATE_PREVIEW = 2;

		/// <summary>
		/// Camera state: Waiting for 3A convergence before capturing a photo.
		/// </summary>
		const int STATE_WAITING_FOR_3A_CONVERGENCE = 3;

		/// <summary>
		/// An {@link OrientationEventListener} used to determine when device rotation has occurred.
		/// This is mainly necessary for when the device is rotated by 180 degrees, in which case
		///	onCreate or onConfigurationChanged is not called as the view dimensions remain the same,
		///	but the orientation of the has changed, and thus the preview rotation must be updated..
		/// </summary>
		OrientationEventListener mOrientationListener;

		class SurfaceTextureListener : Java.Lang.Object, TextureView.ISurfaceTextureListener
		{
			Activity Activity { get; set; }

			public SurfaceTextureListener (Activity activity)
			{
				Activity = activity;
			}

			public void OnSurfaceTextureAvailable (Android.Graphics.SurfaceTexture surface, int width, int height)
			{
				ConfigureTransform (width, height, Activity);
			}

			public bool OnSurfaceTextureDestroyed (Android.Graphics.SurfaceTexture surface)
			{
				lock (mCameraStateLock) {
					mPreviewSize = null;
				}
				return true;
			}

			public void OnSurfaceTextureSizeChanged (Android.Graphics.SurfaceTexture surface, int width, int height)
			{
				ConfigureTransform (width, height, Activity);
			}

			public void OnSurfaceTextureUpdated (Android.Graphics.SurfaceTexture surface)
			{
			}
		}

		/// <summary>
		/// {@link TextureView.SurfaceTextureListener} handles several lifecycle events of a
		/// {@link TextureView}.
		/// </summary>
		TextureView.ISurfaceTextureListener mSurfaceTextureListener;

		/// <summary>
		/// An {@link AutoFitTextureView} for camera preview.
		/// </summary>
		static AutoFitTextureView mTextureView;

		/// <summary>
		/// An additional thread for running tasks that shouldn't block the UI.  This is used for all
		/// callbacks from the {@link CameraDevice} and {@link CameraCaptureSession}s.
		/// </summary>
		HandlerThread mBackgroundThread;

		/// <summary>
		/// A counter for tracking corresponding {@link CaptureRequest}s and {@link CaptureResult}s
		/// across the {@link CameraCaptureSession} capture callbacks.
		/// </summary>
		static readonly AtomicInteger mRequestCounter = new AtomicInteger ();

		/// <summary>
		/// A {@link Semaphore} to prevent the app from exiting before closing the camera.
		/// </summary>
		static readonly Semaphore mCameraOpenCloseLock = new Semaphore (1);

		/// <summary>
		/// A lock protecting camera state.
		/// </summary>
		static readonly object mCameraStateLock = new object ();

		// *********************************************************************************************
		// State protected by mCameraStateLock.
		//
		// The following state is used across both the UI and background threads.  Methods with "Locked"
		// in the name expect mCameraStateLock to be held while calling.

		/// <summary>
		/// ID of the current {@link CameraDevice}.
		/// </summary>
		string mCameraId;

		/// <summary>
		/// A {@link CameraCaptureSession } for camera preview.
		/// </summary>
		static CameraCaptureSession mCaptureSession;

		/// <summary>
		/// A reference to the open {@link CameraDevice}.
		/// </summary>
		static CameraDevice mCameraDevice;

		/// <summary>
		/// The {@link Size} of camera preview.
		/// </summary>
		static Size mPreviewSize;

		/// <summary>
		/// The {@link CameraCharacteristics} for the currently configured camera device.
		/// </summary>
		static CameraCharacteristics mCharacteristics;

		/// <summary>
		/// A {@link Handler} for running tasks in the background.
		/// </summary>
		static Handler mBackgroundHandler;

		/// <summary>
		/// A reference counted holder wrapping the {@link ImageReader} that handles JPEG image captures.
		/// This is used to allow us to clean up the {@link ImageReader} when all background tasks using
		/// its {@link Image}s have completed.
		/// </summary>
		static RefCountedAutoCloseable<ImageReader> mJpegImageReader;

		/// <summary>
		/// A reference counted holder wrapping the {@link ImageReader} that handles RAW image captures.
		/// This is used to allow us to clean up the {@link ImageReader} when all background tasks using
		/// its {@link Image}s have completed.
		/// </summary>
		static RefCountedAutoCloseable<ImageReader> mRawImageReader;

		/// <summary>
		/// Whether or not the currently configured camera device is fixed-focus.
		/// </summary>
		static bool mNoAFRun = false;

		/// <summary>
		/// Number of pending user requests to capture a photo.
		/// </summary>
		static int mPendingUserCaptures = 0;

		/// <summary>
		/// Request ID to {@link ImageSaver.ImageSaverBuilder} mapping for in-progress JPEG captures.
		/// </summary>
		static readonly TreeMap mJpegResultQueue = new TreeMap ();

		/// <summary>
		/// Request ID to {@link ImageSaver.ImageSaverBuilder} mapping for in-progress RAW captures.
		/// </summary>
		static readonly TreeMap mRawResultQueue = new TreeMap ();

		/// <summary>
		/// {@link CaptureRequest.Builder} for the camera preview
		/// </summary>
		static CaptureRequest.Builder mPreviewRequestBuilder;

		/// <summary>
		/// The state of the camera device.
		/// 
		/// @see #mPreCaptureCallback
		/// </summary>
		static int mState = STATE_CLOSED;

		/// <summary>
		/// Timer to use with pre-capture sequence to ensure a timely capture if 3A convergence is taking
		/// too long.
		/// </summary>
		static long mCaptureTimer;

		//**********************************************************************************************

		class StateCallback : CameraDevice.StateCallback
		{
			Activity Activity { get; set; }

			public StateCallback (Activity activity)
			{
				Activity = activity;
			}

			public override void OnDisconnected (CameraDevice camera)
			{
				lock (mCameraStateLock) {
					mState = STATE_CLOSED;
					mCameraOpenCloseLock.Release ();
					camera.Close ();
					mCameraDevice = null;
				}
			}

			public override void OnError (CameraDevice camera, Android.Hardware.Camera2.CameraError error)
			{
				Log.Error (TAG, "Received camera device error: " + error);
				lock (mCameraStateLock) {
					mState = STATE_CLOSED;
					mCameraOpenCloseLock.Release ();
					camera.Close ();
					mCameraDevice = null;
				}
				var activity = Activity;
				if (null != activity) {
					activity.Finish ();
				}
			}

			public override void OnOpened (CameraDevice camera)
			{
				// This method is called when the camera is opened.  We start camera preview here if
				// the TextureView displaying this has been set up.
				lock (mCameraStateLock) {
					mState = STATE_OPENED;
					mCameraOpenCloseLock.Release ();
					mCameraDevice = camera;

					// Start the preview session if the TextureView has been set up already.
					if (mPreviewSize != null && mTextureView.IsAvailable) {
						CreateCameraPreviewSessionLocked ();
					}
				}
			}
		}

		/// <summary>
		/// {@link CameraDevice.StateCallback} is called when the currently active {@link CameraDevice}
		/// changes its state.
		/// </summary>
		CameraDevice.StateCallback mStateCallback;

		class OnJpegImageAvailableListener : Java.Lang.Object, ImageReader.IOnImageAvailableListener
		{
			public void OnImageAvailable (ImageReader reader)
			{
				DequeueAndSaveImage (mJpegResultQueue, mJpegImageReader);
			}
		}

		/// <summary>
		/// This a callback object for the {@link ImageReader}. "onImageAvailable" will be called when a
		/// JPEG image is ready to be saved.
		/// </summary>
		readonly ImageReader.IOnImageAvailableListener mOnJpegImageAvailableListener = new OnJpegImageAvailableListener ();

		class OnRawImageAvailableListener : Java.Lang.Object, ImageReader.IOnImageAvailableListener
		{
			public void OnImageAvailable (ImageReader reader)
			{
				DequeueAndSaveImage (mRawResultQueue, mRawImageReader);
			}
		}

		/// <summary>
		/// This a callback object for the {@link ImageReader}. "onImageAvailable" will be called when a
		/// RAW image is ready to be saved.
		/// </summary>
		readonly ImageReader.IOnImageAvailableListener mOnRawImageAvailableListener = new OnRawImageAvailableListener ();


		class PreCameraCaptureCallback : CameraCaptureSession.CaptureCallback
		{
			Activity Activity { get; set; }

			public PreCameraCaptureCallback (Activity activity)
			{
				Activity = activity;
			}

			void Process (CaptureResult result)
			{
				lock (mCameraStateLock) {
					switch (mState) {
					case STATE_PREVIEW:
						// We have nothing to do when the camera preview is running normally.
						break;
					case STATE_WAITING_FOR_3A_CONVERGENCE:
						bool readyToCapture = true;
						if (!mNoAFRun) {
							int afState = (int)result.Get (CaptureResult.ControlAfState);

							// If auto-focus has reached locked state, we are ready to capture
							readyToCapture = (afState == (int)ControlAFState.FocusedLocked ||
							afState == (int)ControlAFState.NotFocusedLocked);
						}

							// If we are running on an non-legacy device, we should also wait until
							// auto-exposure and auto-white-balance have converged as well before
							// taking a picture.
						if (!IsLegacyLocked ()) {
							var aeState = (int)result.Get (CaptureResult.ControlAeState);
							var awbState = (int)result.Get (CaptureResult.ControlAwbState);

							readyToCapture = readyToCapture &&
							aeState == (int)ControlAEState.Converged &&
							awbState == (int)ControlAwbState.Converged;
						}

							// If we haven't finished the pre-capture sequence but have hit our maximum
							// wait timeout, too bad! Begin capture anyway.
						if (!readyToCapture && HitTimeoutLocked ()) {
							Log.Warn (TAG, "Timed out waiting for pre-capture sequence to complete.");
							readyToCapture = true;
						}

						if (readyToCapture && mPendingUserCaptures > 0) {
							// Capture once for each user tap of the "Picture" button.
							while (mPendingUserCaptures > 0) {
								CaptureStillPictureLocked (Activity);
								mPendingUserCaptures--;
							}
							// After this, the camera will go back to the normal state of preview.
							mState = STATE_PREVIEW;
						}
						break;
					}
				}
			}

			public override void OnCaptureProgressed (CameraCaptureSession session, CaptureRequest request,
			                                          CaptureResult partialResult)
			{
				Process (partialResult);
			}

			public override void OnCaptureCompleted (CameraCaptureSession session, CaptureRequest request,
			                                         TotalCaptureResult result)
			{
				Process (result);
			}
		}

		/// <summary>>
		/// A {@link CameraCaptureSession.CaptureCallback} that handles events for the preview and
		/// pre-capture sequence.
		/// </summary>
		static CameraCaptureSession.CaptureCallback mPreCaptureCallback;

		class CaptureCallback : CameraCaptureSession.CaptureCallback
		{
			public override void OnCaptureStarted (CameraCaptureSession session, CaptureRequest request,
			                                       long timestamp, long frameNumber)
			{
				string currentDateTime = GenerateTimestamp ();
				var path = System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal);
				var rawFilePath = System.IO.Path.Combine (path, "RAW_" + currentDateTime + ".dng");
				var jpegFilePath = System.IO.Path.Combine (path, "JPEG_" + currentDateTime + ".jpg");
				var rawFile = new FileInfo (rawFilePath);
				var jpegFile = new FileInfo (jpegFilePath);

				// Look up the ImageSaverBuilder for this request and update it with the file name
				// based on the capture start time.
				ImageSaver.ImageSaverBuilder jpegBuilder;
				ImageSaver.ImageSaverBuilder rawBuilder;
				int requestId = (int)request.Tag;
				lock (mCameraStateLock) {
					jpegBuilder = (ImageSaver.ImageSaverBuilder)mJpegResultQueue.Get (requestId);
					rawBuilder = (ImageSaver.ImageSaverBuilder)mRawResultQueue.Get (requestId);
				}

				if (jpegBuilder != null)
					jpegBuilder.SetFile (jpegFile);
				
				if (rawBuilder != null)
					rawBuilder.SetFile (rawFile);
			}

			public override void OnCaptureCompleted (CameraCaptureSession session, CaptureRequest request,
			                                         TotalCaptureResult result)
			{
				int requestId = (int)request.Tag;
				ImageSaver.ImageSaverBuilder jpegBuilder;
				ImageSaver.ImageSaverBuilder rawBuilder;
				var sb = new System.Text.StringBuilder ();

				// Look up the ImageSaverBuilder for this request and update it with the CaptureResult
				lock (mCameraStateLock) {
					jpegBuilder = (ImageSaver.ImageSaverBuilder)mJpegResultQueue.Get (requestId);
					rawBuilder = (ImageSaver.ImageSaverBuilder)mRawResultQueue.Get (requestId);

					// If we have all the results necessary, save the image to a file in the background.
					HandleCompletionLocked (requestId, jpegBuilder, mJpegResultQueue);
					HandleCompletionLocked (requestId, rawBuilder, mRawResultQueue);

					if (jpegBuilder != null) {
						jpegBuilder.SetResult (result);
						sb.Append ("Saving JPEG as: ");
						sb.Append (jpegBuilder.GetSaveLocation ());
					}
					if (rawBuilder != null) {
						rawBuilder.SetResult (result);
						if (jpegBuilder != null)
							sb.Append (", ");
						sb.Append ("Saving RAW as: ");
						sb.Append (rawBuilder.GetSaveLocation ());
					}
					FinishedCaptureLocked ();
				}

				ShowToast (sb.ToString ());
			}

			public override void OnCaptureFailed (CameraCaptureSession session, CaptureRequest request,
			                                      CaptureFailure failure)
			{
				int requestId = (int)request.Tag;
				lock (mCameraStateLock) {
					mJpegResultQueue.Remove (requestId);
					mRawResultQueue.Remove (requestId);
					FinishedCaptureLocked ();
				}
				ShowToast ("Capture failed!");
			}
		}

		/// <summary>
		/// A {@link CameraCaptureSession.CaptureCallback} that handles the still JPEG and RAW capture
		/// request.
		/// </summary>
		static readonly CameraCaptureSession.CaptureCallback mCaptureCallback = new CaptureCallback ();

		class MessageHandler : Handler
		{
			Activity Activity { get; set; }

			public MessageHandler (Looper looper, Activity activity) : base (looper)
			{
				Activity = activity;
			}

			public override void HandleMessage (Message msg)
			{
				if (Activity != null) {
					Toast.MakeText (Activity, (string)msg.Obj, ToastLength.Short).Show ();
				}
			}
		}

		/// <summary>
		/// A {@link Handler} for showing {@link Toast}s on the UI thread.
		/// </summary>
		static Handler mMessageHandler;

		public static Camera2RawFragment Create ()
		{
			Camera2RawFragment fragment = new Camera2RawFragment ();
			fragment.RetainInstance = true;
			return fragment;
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container,
		                                   Bundle savedInstanceState)
		{
			return inflater.Inflate (Resource.Layout.fragment_camera2_basic, container, false);
		}

		class OrientationListener : OrientationEventListener
		{
			Activity Activity { get; set; }

			public OrientationListener (Context context, SensorDelay delay, Activity activity) : base (context, delay)
			{
				Activity = activity;
			}

			public override void OnOrientationChanged (int orientation)
			{
				if (mTextureView != null && mTextureView.IsAvailable) {
					ConfigureTransform (mTextureView.Width, mTextureView.Height, Activity);
				}
			}
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			view.FindViewById (Resource.Id.picture).SetOnClickListener (this);
			view.FindViewById (Resource.Id.info).SetOnClickListener (this);
			mTextureView = (AutoFitTextureView)view.FindViewById (Resource.Id.texture);

			// Setup a new OrientationEventListener.  This is used to handle rotation events like a
			// 180 degree rotation that do not normally trigger a call to onCreate to do view re-layout
			// or otherwise cause the preview TextureView's size to change.
			mOrientationListener = new OrientationListener (Activity, SensorDelay.Normal, Activity);
			mMessageHandler = new MessageHandler (Looper.MainLooper, Activity);
			mPreCaptureCallback = new PreCameraCaptureCallback (Activity);
			mSurfaceTextureListener = new SurfaceTextureListener (Activity);
			mStateCallback = new StateCallback (Activity);
		}

		public override void OnResume ()
		{
			base.OnResume ();
			StartBackgroundThread ();

			if (CanOpenCamera ()) {

				// When the screen is turned off and turned back on, the SurfaceTexture is already
				// available, and "onSurfaceTextureAvailable" will not be called. In that case, we should
				// configure the preview bounds here (otherwise, we wait until the surface is ready in
				// the SurfaceTextureListener).
				if (mTextureView.IsAvailable) {
					ConfigureTransform (mTextureView.Width, mTextureView.Height, Activity);
				} else {
					mTextureView.SurfaceTextureListener = mSurfaceTextureListener;
				}
				if (mOrientationListener != null && mOrientationListener.CanDetectOrientation ()) {
					mOrientationListener.Enable ();
				}
			}
		}

		public override void OnPause ()
		{
			if (mOrientationListener != null) {
				mOrientationListener.Disable ();
			}
			CloseCamera ();
			StopBackgroundThread ();
			base.OnPause ();
		}

		public void OnClick (View view)
		{
			switch (view.Id) {
			case Resource.Id.picture:
				{
					TakePicture ();
					break;
				}
			case Resource.Id.info:
				{
					var activity = Activity;
					if (activity != null) {
						new AlertDialog.Builder (activity)
							.SetMessage (Resource.String.intro_message)
							.SetPositiveButton (Android.Resource.String.Ok, default(IDialogInterfaceOnClickListener))
							.Show ();
					}
					break;
				}
			}
		}

		/// <summary>
		/// Sets up state related to camera that is needed before opening a {@link CameraDevice}.
		/// </summary>
		/// <returns><c>true</c>, if up camera outputs was set, <c>false</c> otherwise.</returns>
		bool SetUpCameraOutputs ()
		{
			var activity = Activity;
			CameraManager manager = (CameraManager)activity.GetSystemService (Context.CameraService);
			if (manager == null) {
				ErrorDialog.BuildErrorDialog ("This device doesn't support Camera2 API.").
				Show (FragmentManager, "dialog");
				return false;
			}
			try {
				// Find a CameraDevice that supports RAW captures, and configure state.
				foreach (string cameraId in manager.GetCameraIdList()) {
					CameraCharacteristics characteristics
					= manager.GetCameraCharacteristics (cameraId);

					// We only use a camera that supports RAW in this sample.
					if (!Contains (characteristics.Get (
						    CameraCharacteristics.RequestAvailableCapabilities).ToArray<int> (),
						    (int)RequestAvailableCapabilities.Raw)) {
						continue;
					}

					StreamConfigurationMap map = (StreamConfigurationMap)characteristics.Get (
						                             CameraCharacteristics.ScalerStreamConfigurationMap);

					// For still image captures, we use the largest available size.
					Size[] jpegs = map.GetOutputSizes ((int)ImageFormatType.Jpeg);
					Size largestJpeg = jpegs.OrderByDescending (element => element.Width * element.Height).First ();

					Size[] raws = map.GetOutputSizes ((int)ImageFormatType.RawSensor);
					Size largestRaw = raws.OrderByDescending (element => element.Width * element.Height).First ();

					lock (mCameraStateLock) {
						// Set up ImageReaders for JPEG and RAW outputs.  Place these in a reference
						// counted wrapper to ensure they are only closed when all background tasks
						// using them are finished.
						if (mJpegImageReader == null || mJpegImageReader.GetAndRetain () == null) {
							mJpegImageReader = new RefCountedAutoCloseable<ImageReader> (
								ImageReader.NewInstance (largestJpeg.Width,
									largestJpeg.Height, ImageFormatType.Jpeg, /*maxImages*/5));
						}

						mJpegImageReader.Get ().SetOnImageAvailableListener (
							mOnJpegImageAvailableListener, mBackgroundHandler);

						if (mRawImageReader == null || mRawImageReader.GetAndRetain () == null) {
							mRawImageReader = new RefCountedAutoCloseable<ImageReader> (
								ImageReader.NewInstance (largestRaw.Width,
									largestRaw.Height, ImageFormatType.RawSensor, /*maxImages*/5));
						}
						mRawImageReader.Get ().SetOnImageAvailableListener (
							mOnRawImageAvailableListener, mBackgroundHandler);

						mCharacteristics = characteristics;
						mCameraId = cameraId;
					}
					return true;
				}
			} catch (CameraAccessException e) {
				e.PrintStackTrace ();
			}

			// If we found no suitable cameras for capturing RAW, warn the user.
			ErrorDialog.BuildErrorDialog ("This device doesn't support capturing RAW photos").
			Show (FragmentManager, "dialog");
			return false;
		}

		/// <summary>
		/// Opens the camera specified by {@link #mCameraId}.
		/// </summary>
		bool CanOpenCamera ()
		{
			if (!SetUpCameraOutputs ())
				return false;
			
			var activity = Activity;
			CameraManager manager = (CameraManager)activity.GetSystemService (Context.CameraService);
			try {
				// Wait for any previously running session to finish.
				if (!mCameraOpenCloseLock.TryAcquire (2500, TimeUnit.Milliseconds))
					throw new RuntimeException ("Time out waiting to lock camera opening.");

				string cameraId;
				Handler backgroundHandler;
				lock (mCameraStateLock) {
					cameraId = mCameraId;
					backgroundHandler = mBackgroundHandler;
				}

				// Attempt to open the camera. mStateCallback will be called on the background handler's
				// thread when this succeeds or fails.
				manager.OpenCamera (cameraId, mStateCallback, backgroundHandler);
			} catch (CameraAccessException e) {
				e.PrintStackTrace ();
			} catch (InterruptedException e) {
				throw new RuntimeException ("Interrupted while trying to lock camera opening.", e);
			}
			return true;
		}

		/// <summary>
		/// Closes the current {@link CameraDevice}.
		/// </summary>
		void CloseCamera ()
		{
			try {
				mCameraOpenCloseLock.Acquire ();
				lock (mCameraStateLock) {

					// Reset state and clean up resources used by the camera.
					// Note: After calling this, the ImageReaders will be closed after any background
					// tasks saving Images from these readers have been completed.
					mPendingUserCaptures = 0;
					mState = STATE_CLOSED;
					if (null != mCaptureSession) {
						mCaptureSession.Close ();
						mCaptureSession = null;
					}
					if (null != mCameraDevice) {
						mCameraDevice.Close ();
						mCameraDevice = null;
					}
					if (null != mJpegImageReader) {
						mJpegImageReader.Close ();
						mJpegImageReader = null;
					}
					if (null != mRawImageReader) {
						mRawImageReader.Close ();
						mRawImageReader = null;
					}
				}
			} catch (InterruptedException e) {
				throw new RuntimeException ("Interrupted while trying to lock camera closing.", e);
			} finally {
				mCameraOpenCloseLock.Release ();
			}
		}

		/// <summary>
		/// Starts a background thread and its {@link Handler}.
		/// </summary>
		void StartBackgroundThread ()
		{
			mBackgroundThread = new HandlerThread ("CameraBackground");
			mBackgroundThread.Start ();
			lock (mCameraStateLock) {
				mBackgroundHandler = new Handler (mBackgroundThread.Looper);
			}
		}

		/// <summary>
		/// Stops the background thread and its {@link Handler}.
		/// </summary>
		void StopBackgroundThread ()
		{
			mBackgroundThread.QuitSafely ();
			try {
				mBackgroundThread.Join ();
				mBackgroundThread = null;
				lock (mCameraStateLock) {
					mBackgroundHandler = null;
				}
			} catch (InterruptedException e) {
				e.PrintStackTrace ();
			}
		}

		class CameraPreviewCaptureCallback : CameraCaptureSession.StateCallback
		{
			public override void OnConfigured (CameraCaptureSession cameraCaptureSession)
			{
				lock (mCameraStateLock) {
					// The camera is already closed
					if (null == mCameraDevice)
						return;

					try {
						Setup3AControlsLocked (mPreviewRequestBuilder);
						// Finally, we start displaying the camera preview.
						cameraCaptureSession.SetRepeatingRequest (
							mPreviewRequestBuilder.Build (),
							mPreCaptureCallback, mBackgroundHandler);
						mState = STATE_PREVIEW;
					} catch (CameraAccessException e) {
						e.PrintStackTrace ();
						return;
					} catch (IllegalStateException e) {
						e.PrintStackTrace ();
						return;
					}
					// When the session is ready, we start displaying the preview.
					mCaptureSession = cameraCaptureSession;
				}
			}

			public override void OnConfigureFailed (CameraCaptureSession session)
			{
				ShowToast ("Failed to configure camera.");
			}
		}

		/// <summary>
		/// Creates a new {@link CameraCaptureSession} for camera preview.
		///
		/// Call this only with {@link #mCameraStateLock} held.
		/// </summary>
		static void CreateCameraPreviewSessionLocked ()
		{
			try {
				SurfaceTexture texture = mTextureView.SurfaceTexture;
				// We configure the size of default buffer to be the size of camera preview we want.
				texture.SetDefaultBufferSize (mPreviewSize.Width, mPreviewSize.Height);

				// This is the output Surface we need to start preview.
				Surface surface = new Surface (texture);

				// We set up a CaptureRequest.Builder with the output Surface.
				mPreviewRequestBuilder
				= mCameraDevice.CreateCaptureRequest (CameraTemplate.Preview);
				mPreviewRequestBuilder.AddTarget (surface);

				// Here, we create a CameraCaptureSession for camera preview.
				mCameraDevice.CreateCaptureSession (new List<Surface> () {surface,
					mJpegImageReader.Get ().Surface,
					mRawImageReader.Get ().Surface
				}, new CameraPreviewCaptureCallback (), mBackgroundHandler);
			} catch (CameraAccessException e) {
				e.PrintStackTrace ();
			}
		}

		/// <summary>
		/// Configure the given {@link CaptureRequest.Builder} to use auto-focus, auto-exposure, and
		/// auto-white-balance controls if available.
		/// 
		/// Call this only with {@link #mCameraStateLock} held.
		/// </summary>
		/// <param name="builder">the builder to configure.</param>
		static void Setup3AControlsLocked (CaptureRequest.Builder builder)
		{
			// Enable auto-magical 3A run by camera device
			builder.Set (CaptureRequest.ControlMode, (int)ControlMode.Auto);

			var minFocusDist = (float)mCharacteristics.Get (CameraCharacteristics.LensInfoMinimumFocusDistance);

			// If MINIMUM_FOCUS_DISTANCE is 0, lens is fixed-focus and we need to skip the AF run.
			mNoAFRun = (minFocusDist == null || minFocusDist == 0);

			if (!mNoAFRun) {
				// If there is a "continuous picture" mode available, use it, otherwise default to AUTO.
				if (Contains (mCharacteristics.Get (
					    CameraCharacteristics.ControlAfAvailableModes).ToArray<int> (),
					    (int)ControlAFMode.ContinuousPicture)) {
					builder.Set (CaptureRequest.ControlAfMode, (int)ControlAFMode.ContinuousPicture);
				} else {
					builder.Set (CaptureRequest.ControlAfMode, (int)ControlAFMode.Auto);
				}
			}

			// If there is an auto-magical flash control mode available, use it, otherwise default to
			// the "on" mode, which is guaranteed to always be available.
			if (Contains (mCharacteristics.Get (
				    CameraCharacteristics.ControlAeAvailableModes).ToArray<int> (), (int)ControlAEMode.OnAutoFlash)) {
				builder.Set (CaptureRequest.ControlAeMode, (int)ControlAEMode.OnAutoFlash);
			} else {
				builder.Set (CaptureRequest.ControlAeMode, (int)ControlAEMode.On);
			}

			// If there is an auto-magical white balance control mode available, use it.
			if (Contains (mCharacteristics.Get (
				    CameraCharacteristics.ControlAwbAvailableModes).ToArray<int> (), (int)ControlAwbMode.Auto)) {
				// Allow AWB to run auto-magically if this device supports this
				builder.Set (CaptureRequest.ControlAwbMode, (int)ControlAwbMode.Auto);
			}
		}

		/// <summary>
		/// Configure the necessary {@link android.graphics.Matrix} transformation to `mTextureView`,
		/// and start/restart the preview capture session if necessary.
		///
		/// This method should be called after the camera state has been initialized in
		/// setUpCameraOutputs.
		/// </summary>
		/// <param name="viewWidth">The width of `mTextureView`</param>
		/// <param name="viewHeight">The height of `mTextureView`</param>
		static void ConfigureTransform (int viewWidth, int viewHeight, Activity activity)
		{
			lock (mCameraStateLock) {
				if (mTextureView == null || activity == null) {
					return;
				}

				var map = (StreamConfigurationMap)mCharacteristics.Get (CameraCharacteristics.ScalerStreamConfigurationMap);

				// For still image captures, we always use the largest available size.
				Size largestJpeg = (Size)Collections.Max (Arrays.AsList (map.GetOutputSizes ((int)ImageFormatType.Jpeg)),
					                   new CompareSizesByArea ());

				// Find the rotation of the device relative to the native device orientation.
				var deviceRotation = (int)activity.WindowManager.DefaultDisplay.Rotation;

				// Find the rotation of the device relative to the camera sensor's orientation.
				int totalRotation = SensorToDeviceRotation (mCharacteristics, deviceRotation);

				// Swap the view dimensions for calculation as needed if they are rotated relative to
				// the sensor.
				bool swappedDimensions = totalRotation == 90 || totalRotation == 270;
				int rotatedViewWidth = viewWidth;
				int rotatedViewHeight = viewHeight;
				if (swappedDimensions) {
					rotatedViewWidth = viewHeight;
					rotatedViewHeight = viewWidth;
				}

				// Find the best preview size for these view dimensions and configured JPEG size.
				Size previewSize = ChooseOptimalSize (map.GetOutputSizes (Class.FromType (typeof(SurfaceTexture))),
					                   rotatedViewWidth, rotatedViewHeight, largestJpeg);

				if (swappedDimensions) {
					mTextureView.SetAspectRatio (
						previewSize.Height, previewSize.Width);
				} else {
					mTextureView.SetAspectRatio (
						previewSize.Width, previewSize.Height);
				}

				// Find rotation of device in degrees (reverse device orientation for front-facing
				// cameras).
				int rotation = ((int)mCharacteristics.Get (CameraCharacteristics.LensFacing) ==
				               (int)LensFacing.Front) ?
					(360 + ORIENTATIONS.Get (deviceRotation)) % 360 :
					(360 - ORIENTATIONS.Get (deviceRotation)) % 360;

				Matrix matrix = new Matrix ();
				RectF viewRect = new RectF (0, 0, viewWidth, viewHeight);
				RectF bufferRect = new RectF (0, 0, previewSize.Height, previewSize.Width);
				float centerX = viewRect.CenterX ();
				float centerY = viewRect.CenterY ();

				// Initially, output stream images from the Camera2 API will be rotated to the native
				// device orientation from the sensor's orientation, and the TextureView will default to
				// scaling these buffers to fill it's view bounds.  If the aspect ratios and relative
				// orientations are correct, this is fine.
				//
				// However, if the device orientation has been rotated relative to its native
				// orientation so that the TextureView's dimensions are swapped relative to the
				// native device orientation, we must do the following to ensure the output stream
				// images are not incorrectly scaled by the TextureView:
				//   - Undo the scale-to-fill from the output buffer's dimensions (i.e. its dimensions
				//     in the native device orientation) to the TextureView's dimension.
				//   - Apply a scale-to-fill from the output buffer's rotated dimensions
				//     (i.e. its dimensions in the current device orientation) to the TextureView's
				//     dimensions.
				//   - Apply the rotation from the native device orientation to the current device
				//     rotation.
				if (deviceRotation == (int)SurfaceOrientation.Rotation90 || deviceRotation == (int)SurfaceOrientation.Rotation270) {
					bufferRect.Offset (centerX - bufferRect.CenterX (), centerY - bufferRect.CenterY ());
					matrix.SetRectToRect (viewRect, bufferRect, Matrix.ScaleToFit.Fill);
					float scale = System.Math.Max (
						              (float)viewHeight / previewSize.Height,
						              (float)viewWidth / previewSize.Width);
					matrix.PostScale (scale, scale, centerX, centerY);

				}
				matrix.PostRotate (rotation, centerX, centerY);

				mTextureView.SetTransform (matrix);

				// Start or restart the active capture session if the preview was initialized or
				// if its aspect ratio changed significantly.
				if (mPreviewSize == null || !CheckAspectsEqual (previewSize, mPreviewSize)) {
					mPreviewSize = previewSize;
					if (mState != STATE_CLOSED) {
						CreateCameraPreviewSessionLocked ();
					}
				}
			}
		}

		/// <summary>
		/// Initiate a still image capture.
		///
		/// This function sends a capture request that initiates a pre-capture sequence in our state
		/// machine that waits for auto-focus to finish, ending in a "locked" state where the lens is no
		/// longer moving, waits for auto-exposure to choose a good exposure value, and waits for
		/// auto-white-balance to converge.
		/// </summary>
		void TakePicture ()
		{
			lock (mCameraStateLock) {
				mPendingUserCaptures++;

				// If we already triggered a pre-capture sequence, or are in a state where we cannot
				// do this, return immediately.
				if (mState != STATE_PREVIEW) {
					return;
				}

				try {
					// Trigger an auto-focus run if camera is capable. If the camera is already focused,
					// this should do nothing.
					if (!mNoAFRun) {
						mPreviewRequestBuilder.Set (CaptureRequest.ControlAfTrigger,
							(int)ControlAFTrigger.Start);
					}

					// If this is not a legacy device, we can also trigger an auto-exposure metering
					// run.
					if (!IsLegacyLocked ()) {
						// Tell the camera to lock focus.
						mPreviewRequestBuilder.Set (CaptureRequest.ControlAePrecaptureTrigger,
							(int)ControlAEPrecaptureTrigger.Start);
					}

					// Update state machine to wait for auto-focus, auto-exposure, and
					// auto-white-balance (aka. "3A") to converge.
					mState = STATE_WAITING_FOR_3A_CONVERGENCE;

					// Start a timer for the pre-capture sequence.
					StartTimerLocked ();

					// Replace the existing repeating request with one with updated 3A triggers.
					mCaptureSession.Capture (mPreviewRequestBuilder.Build (), mPreCaptureCallback,
						mBackgroundHandler);
				} catch (CameraAccessException e) {
					e.PrintStackTrace ();
				}
			}
		}

		/// <summary>
		/// Send a capture request to the camera device that initiates a capture targeting the JPEG and
		/// RAW outputs.
		///
		/// Call this only with {@link #mCameraStateLock} held.
		/// </summary>
		static void CaptureStillPictureLocked (Activity activity)
		{
			try {
				if (null == activity || null == mCameraDevice)
					return;

				// This is the CaptureRequest.Builder that we use to take a picture.
				CaptureRequest.Builder captureBuilder =
					mCameraDevice.CreateCaptureRequest (CameraTemplate.StillCapture);

				captureBuilder.AddTarget (mJpegImageReader.Get ().Surface);
				captureBuilder.AddTarget (mRawImageReader.Get ().Surface);

				// Use the same AE and AF modes as the preview.
				Setup3AControlsLocked (captureBuilder);

				// Set orientation.
				var rotation = activity.WindowManager.DefaultDisplay.Rotation;
				captureBuilder.Set (CaptureRequest.JpegOrientation, SensorToDeviceRotation (mCharacteristics, (int)rotation));

				// Set request tag to easily track results in callbacks.
				captureBuilder.SetTag (mRequestCounter.IncrementAndGet ());

				CaptureRequest request = captureBuilder.Build ();

				// Create an ImageSaverBuilder in which to collect results, and add it to the queue
				// of active requests.
				ImageSaver.ImageSaverBuilder jpegBuilder = new ImageSaver.ImageSaverBuilder (activity)
					.SetCharacteristics (mCharacteristics);
				ImageSaver.ImageSaverBuilder rawBuilder = new ImageSaver.ImageSaverBuilder (activity)
					.SetCharacteristics (mCharacteristics);

				mJpegResultQueue.Put ((int)request.Tag, jpegBuilder);
				mRawResultQueue.Put ((int)request.Tag, rawBuilder);

				mCaptureSession.Capture (request, mCaptureCallback, mBackgroundHandler);

			} catch (CameraAccessException e) {
				e.PrintStackTrace ();
			}
		}

		/// <summary>
		/// Called after a RAW/JPEG capture has completed; resets the AF trigger state for the
		/// pre-capture sequence.
		///
		/// Call this only with {@link #mCameraStateLock} held.
		/// </summary>
		static void FinishedCaptureLocked ()
		{
			try {
				// Reset the auto-focus trigger in case AF didn't run quickly enough.
				if (!mNoAFRun) {
					mPreviewRequestBuilder.Set (CaptureRequest.ControlAfTrigger,
						(int)ControlAFTrigger.Cancel);

					mCaptureSession.Capture (mPreviewRequestBuilder.Build (),
						mPreCaptureCallback, mBackgroundHandler);

					mPreviewRequestBuilder.Set (CaptureRequest.ControlAfTrigger,
						(int)ControlAFTrigger.Idle);
				}
			} catch (CameraAccessException e) {
				e.PrintStackTrace ();
			}
		}

		/// <summary>
		/// Retrieve the next {@link Image} from a reference counted {@link ImageReader}, retaining
		/// that {@link ImageReader} until that {@link Image} is no longer in use, and set this
		/// {@link Image} as the result for the next request in the queue of pending requests.  If
		/// all necessary information is available, begin saving the image to a file in a background
		/// thread.
		/// </summary>
		/// <param name="pendingQueue">the currently active requests.</param>
		/// <param name="reader">a reference counted wrapper containing an {@link ImageReader} from which to acquire an image.</param>
		static void DequeueAndSaveImage (TreeMap pendingQueue,
		                                 RefCountedAutoCloseable<ImageReader> reader)
		{
			lock (mCameraStateLock) {
				IMapEntry entry = pendingQueue.FirstEntry ();
				var builder = (ImageSaver.ImageSaverBuilder)entry.Value;

				// Increment reference count to prevent ImageReader from being closed while we
				// are saving its Images in a background thread (otherwise their resources may
				// be freed while we are writing to a file).
				if (reader == null || reader.GetAndRetain () == null) {
					Log.Error (TAG, "Paused the activity before we could save the image," +
					" ImageReader already closed.");
					pendingQueue.Remove (entry.Key);
					return;
				}

				Image image;
				try {
					image = reader.Get ().AcquireNextImage ();
				} catch (IllegalStateException) {
					Log.Error (TAG, "Too many images queued for saving, dropping image for request: " +
					entry.Key);
					pendingQueue.Remove (entry.Key);
					return;
				}

				builder.SetRefCountedReader (reader).SetImage (image);

				HandleCompletionLocked ((int)entry.Key, builder, pendingQueue);
			}
		}

		/// <summary>
		/// Runnable that saves an {@link Image} into the specified {@link File}, and updates
		/// {@link android.provider.MediaStore} to include the resulting file.
		///
		/// This can be constructed through an {@link ImageSaverBuilder} as the necessary image and
		/// result information becomes available.
		/// </summary>
		class ImageSaver : Java.Lang.Object, IRunnable
		{
			/// <summary>
			/// The image to save.
			/// </summary>
			readonly Image mImage;

			/// <summary>
			/// The file we save the image into.
			/// </summary>
			readonly FileInfo mFile;

			/// <summary>
			/// The CaptureResult for this image capture.
			/// </summary>
			readonly CaptureResult mCaptureResult;

			/// <summary>
			/// The CameraCharacteristics for this camera device.
			/// </summary>
			readonly CameraCharacteristics mCharacteristics;

			/// <summary>
			/// The Context to use when updating MediaStore with the saved images.
			/// </summary>
			readonly Context mContext;

			/// <summary>
			/// A reference counted wrapper for the ImageReader that owns the given image.
			/// </summary>
			readonly RefCountedAutoCloseable<ImageReader> mReader;

			ImageSaver (Image image, FileInfo file, CaptureResult result,
			            CameraCharacteristics characteristics, Context context,
			            RefCountedAutoCloseable<ImageReader> reader)
			{
				mImage = image;
				mFile = file;
				mCaptureResult = result;
				mCharacteristics = characteristics;
				mContext = context;
				mReader = reader;
			}

			public void Run ()
			{
				bool success = false;
				var format = mImage.Format;
				switch (format) {
				case ImageFormatType.Jpeg:
					{
						ByteBuffer buffer = mImage.GetPlanes () [0].Buffer;
						byte[] bytes = new byte[buffer.Remaining ()];
						buffer.Get (bytes);
						FileStream output = null;
						try {
							output = mFile.OpenWrite ();
							output.Write (bytes, 0, bytes.Length);
							success = true;
						} catch (IOException e) {
							Log.Error (TAG, e.Message);
						} finally {
							mImage.Close ();
							CloseOutput (output);
						}
						break;
					}
				case ImageFormatType.RawSensor:
					{
						DngCreator dngCreator = new DngCreator (mCharacteristics, mCaptureResult);
						FileStream output = null;
						try {
							output = mFile.OpenWrite ();
							dngCreator.WriteImage (output, mImage);
							success = true;
						} catch (IOException e) {
							Log.Error (TAG, e.Message);
						} finally {
							mImage.Close ();
							CloseOutput (output);
						}
						break;
					}
				default:
					{
						Log.Error (TAG, "Cannot save image, unexpected image format:" + format);
						break;
					}
				}

				// Decrement reference count to allow ImageReader to be closed to free up resources.
				mReader.Close ();

				// If saving the file succeeded, update MediaStore.
				if (success) {
					MediaScannerConnection.ScanFile (mContext, new string[] { mFile.FullName },
						/*mimeTypes*/null, new MediaScannerClient ());
				}
			}

			class MediaScannerClient : Java.Lang.Object, MediaScannerConnection.IOnScanCompletedListener
			{
				public void OnMediaScannerConnected ()
				{
					//do nothing
				}

				public void OnScanCompleted (string path, Android.Net.Uri uri)
				{
					Log.Info (TAG, "Scanned " + path + ":");
					Log.Info (TAG, "-> uri=" + uri);
				}
			}

			/// <summary>
			/// Builder class for constructing {@link ImageSaver}s.
			///
			/// This class is thread safe.
			/// </summary>
			public class ImageSaverBuilder : Java.Lang.Object
			{
				Image mImage;
				FileInfo mFile;
				CaptureResult mCaptureResult;
				CameraCharacteristics mCharacteristics;
				Context mContext;
				RefCountedAutoCloseable<ImageReader> mReader;

				/// <summary>
				/// Construct a new ImageSaverBuilder using the given {@link Context}.
				/// @param context a {@link Context} to for accessing the
				///                  {@link android.provider.MediaStore}.
				/// </summary>
				public ImageSaverBuilder (Context context)
				{
					mContext = context;
				}

				public ImageSaverBuilder SetRefCountedReader (
					RefCountedAutoCloseable<ImageReader> reader)
				{
					if (reader == null)
						throw new NullPointerException ();

					mReader = reader;
					return this;
				}

				public ImageSaverBuilder SetImage (Image image)
				{
					if (image == null)
						throw new NullPointerException ();
					mImage = image;
					return this;
				}

				public ImageSaverBuilder SetFile (FileInfo file)
				{
					if (file == null)
						throw new NullPointerException ();
					mFile = file;
					return this;
				}

				public ImageSaverBuilder SetResult (CaptureResult result)
				{
					if (result == null)
						throw new NullPointerException ();
					mCaptureResult = result;
					return this;
				}

				public ImageSaverBuilder SetCharacteristics (
					CameraCharacteristics characteristics)
				{
					if (characteristics == null)
						throw new NullPointerException ();
					mCharacteristics = characteristics;
					return this;
				}

				public ImageSaver buildIfComplete ()
				{
					if (!IsComplete) {
						return null;
					}
					return new ImageSaver (mImage, mFile, mCaptureResult, mCharacteristics, mContext,
						mReader);
				}

				public string GetSaveLocation ()
				{
					return (mFile == null) ? "Unknown" : mFile.ToString ();
				}

				bool IsComplete {
					get { 
						return mImage != null && mFile != null && mCaptureResult != null
						&& mCharacteristics != null;
					}
				}
			}
		}

		// Utility classes and methods:
		// *********************************************************************************************

		/// <summary>
		/// Comparator based on area of the given {@link Size} objects.
		/// </summary>
		class CompareSizesByArea : Java.Lang.Object, IComparator
		{
			public int Compare (Size lhs, Size rhs)
			{
				// We cast here to ensure the multiplications won't overflow
				return Long.Signum ((long)lhs.Width * lhs.Height -
				(long)rhs.Width * rhs.Height);
			}

			int IComparator.Compare (Java.Lang.Object lhs, Java.Lang.Object rhs)
			{
				return 0;
			}

			bool IComparator.Equals (Java.Lang.Object @object)
			{
				return false;
			}
		}

		/// <summary>
		/// A dialog fragment for displaying non-recoverable errors; this {@link Activity} will be
		/// finished once the dialog has been acknowledged by the user. 
		/// </summary>
		public class ErrorDialog : DialogFragment
		{

			string mErrorMessage;

			public ErrorDialog ()
			{
				mErrorMessage = "Unknown error occurred!";
			}

			// Build a dialog with a custom message (Fragments require default constructor).
			public static ErrorDialog BuildErrorDialog (string errorMessage)
			{
				ErrorDialog dialog = new ErrorDialog ();
				dialog.mErrorMessage = errorMessage;
				return dialog;
			}

			public override Dialog OnCreateDialog (Bundle savedInstanceState)
			{
				var activity = Activity;
				return new AlertDialog.Builder (activity)
					.SetMessage (mErrorMessage)
					.SetPositiveButton (Android.Resource.String.Ok, new EventHandler<DialogClickEventArgs> ((s, args) => {
					Activity.Finish ();
				})).Create ();
			}


		}

		/// <summary>
		/// A wrapper for an {@link AutoCloseable} object that implements reference counting to allow
		/// for resource management.
		/// </summary>
		public class RefCountedAutoCloseable<T> : Java.Lang.Object, IAutoCloseable where T : Java.Lang.Object
		{
			T mObject;
			long mRefCount = 0;

			/// <summary>
			/// Wrap the given object.
			/// </summary>
			/// <param name="obj">object an object to wrap.</param>
			public RefCountedAutoCloseable (T obj)
			{
				if (obj == null)
					throw new NullPointerException ();
				
				mObject = obj;
			}

			/// <summary>
			/// the reference count and return the wrapped object.
			/// </summary>
			/// <returns>the wrapped object, or null if the object has been released.</returns>
			public T GetAndRetain ()
			{
				if (mRefCount < 0)
					return default(T);

				mRefCount++;
				return mObject;
			}

			/// <summary>
			/// Return the wrapped object.
			/// </summary>
			/// <returns>the wrapped object, or null if the object has been released.</returns>
			public T Get ()
			{
				return mObject;
			}

			/// <summary>
			/// Decrement the reference count and release the wrapped object if there are no other
			/// users retaining this object.
			/// </summary>
			public void Close ()
			{
				if (mRefCount >= 0) {
					mRefCount--;
					if (mRefCount < 0) {
						try {
							var obj = (mObject as IAutoCloseable);
							if (obj == null)
								throw new Java.Lang.Exception ("unclosable");
							obj.Close ();
						} catch (Java.Lang.Exception e) {
							if (e.Message != "unclosable")
								throw new RuntimeException (e);
						} finally {
							mObject = default(T);
						}
					}
				}
			}
		}

		/// <summary>
		/// Given {@code choices} of {@code Size}s supported by a camera, chooses the smallest one whose
		/// width and height are at least as large as the respective requested values, and whose aspect
		/// ratio matches with the specified value.
		/// </summary>
		/// <returns>The optimal {@code Size}, or an arbitrary one if none were big enough</returns>
		/// <param name="choices">The list of sizes that the camera supports for the intended output class</param>
		/// <param name="width">The minimum desired width</param>
		/// <param name="height">The minimum desired height</param>
		/// <param name="aspectRatio">The aspect ratio</param>
		static Size ChooseOptimalSize (Size[] choices, int width, int height, Size aspectRatio)
		{
			// Collect the supported resolutions that are at least as big as the preview Surface
			List<Size> bigEnough = new List<Size> ();
			int w = aspectRatio.Width;
			int h = aspectRatio.Height;
			foreach (Size option in choices) {
				if (option.Height == option.Width * h / w &&
				    option.Width >= width && option.Height >= height) {
					bigEnough.Add (option);
				}
			}

			// Pick the smallest of those, assuming we found any
			if (bigEnough.Count > 0) {
				return (Size)Collections.Min (bigEnough, new CompareSizesByArea ());
			} else {
				Log.Error (TAG, "Couldn't find any suitable preview size");
				return choices [0];
			}
		}

		/// <summary>
		/// Generate a string containing a formatted timestamp with the current date and time.
		/// </summary>
		/// <returns>a {@link String} representing a time.</returns>
		static string GenerateTimestamp ()
		{
			SimpleDateFormat sdf = new SimpleDateFormat ("yyyy_MM_dd_HH_mm_ss_SSS", Java.Util.Locale.Us);
			return sdf.Format (new Date ());
		}

		/// <summary>
		/// Cleanup the given {@link OutputStream}.
		/// </summary>
		/// <param name="outputStream">the stream to close.</param>
		static void CloseOutput (System.IO.Stream outputStream)
		{
			if (outputStream != null) {
				try {
					outputStream.Close ();
				} catch (IOException e) {
					Log.Error (TAG, e.Message);
				}
			}
		}

		/// <summary>
		/// Return true if the given array contains the given integer.
		/// </summary>
		/// <returns><c>true</c>, if the array contains the given integer, <c>false</c> otherwise.</returns>
		/// <param name="modes">array to check.</param>
		/// <param name="mode">integer to get for.</param>
		static bool Contains (int[] modes, int mode)
		{
			if (modes == null) {
				return false;
			}
			foreach (int i in modes) {
				if (i == mode) {
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Return true if the two given {@link Size}s have the same aspect ratio.
		/// </summary>
		/// <returns><c>true</c>, if the sizes have the same aspect ratio, <c>false</c> otherwise.</returns>
		/// <param name="a">first {@link Size} to compare.</param>
		/// <param name="b">second {@link Size} to compare.</param>
		static bool CheckAspectsEqual (Size a, Size b)
		{
			double aAspect = a.Width / (double)a.Height;
			double bAspect = b.Width / (double)b.Height;
			return System.Math.Abs (aAspect - bAspect) <= ASPECT_RATIO_TOLERANCE;
		}

		/// <summary>
		/// Rotation need to transform from the camera sensor orientation to the device's current
		/// orientation.
		/// </summary>
		/// <returns>the total rotation from the sensor orientation to the current device orientation.</returns>
		/// <param name="c">the {@link CameraCharacteristics} to query for the camera sensor orientation.</param>
		/// <param name="deviceOrientation">the current device orientation relative to the native device orientation.</param>
		static int SensorToDeviceRotation (CameraCharacteristics c, int deviceOrientation)
		{
			int sensorOrientation = (int)c.Get (CameraCharacteristics.SensorOrientation);

			// Get device orientation in degrees
			deviceOrientation = ORIENTATIONS.Get (deviceOrientation);

			// Reverse device orientation for front-facing cameras
			if ((int)c.Get (CameraCharacteristics.LensFacing) == (int)LensFacing.Front) {
				deviceOrientation = -deviceOrientation;
			}

			// Calculate desired JPEG orientation relative to camera orientation to make
			// the image upright relative to the device orientation
			return (sensorOrientation + deviceOrientation + 360) % 360;
		}

		/// <summary>
		/// Shows a {@link Toast} on the UI thread.
		/// </summary>
		/// <param name="text">The message to show.</param>
		static void ShowToast (string text)
		{
			// We show a Toast by sending request message to mMessageHandler. This makes sure that the
			// Toast is shown on the UI thread.
			Message message = Message.Obtain ();
			message.Obj = text;
			mMessageHandler.SendMessage (message);
		}

		/// <summary>
		/// If the given request has been completed, remove it from the queue of active requests and
		/// send an {@link ImageSaver} with the results from this request to a background thread to
		/// save a file.
		///
		/// Call this only with {@link #mCameraStateLock} held.
		/// </summary>
		/// <param name="requestId">the ID of the {@link CaptureRequest} to handle.</param>
		/// <param name="builder">the {@link ImageSaver.ImageSaverBuilder} for this request.</param>
		/// <param name="queue">the queue to remove this request from, if completed.</param>
		static void HandleCompletionLocked (int requestId, ImageSaver.ImageSaverBuilder builder, TreeMap queue)
		{
			if (builder == null)
				return;
			ImageSaver saver = builder.buildIfComplete ();
			if (saver != null) {
				queue.Remove (requestId);
				AsyncTask.ThreadPoolExecutor.Execute (saver);
			}
		}

		/// <summary>
		/// Check if we are using a device that only supports the LEGACY hardware level.
		///
		/// Call this only with {@link #mCameraStateLock} held.
		/// </summary>
		/// <returns><c>true</c> if this is a legacy device; otherwise, <c>false</c>.</returns>
		static bool IsLegacyLocked ()
		{
			return (int)mCharacteristics.Get (CameraCharacteristics.InfoSupportedHardwareLevel) == (int)InfoSupportedHardwareLevel.Legacy;
		}

		/// <summary>
		/// Start the timer for the pre-capture sequence.
		///
		/// Call this only with {@link #mCameraStateLock} held.
		/// </summary>
		static void StartTimerLocked ()
		{
			mCaptureTimer = SystemClock.ElapsedRealtime ();
		}

		/// <summary>
		/// Check if the timer for the pre-capture sequence has been hit.
		///
		/// Call this only with {@link #mCameraStateLock} held.
		/// </summary>
		/// <returns><c>true</c>, if the timeout occurred, <c>false</c> otherwise.</returns>
		static bool HitTimeoutLocked ()
		{
			return (SystemClock.ElapsedRealtime () - mCaptureTimer) > PRECAPTURE_TIMEOUT_MS;
		}
	}
}

