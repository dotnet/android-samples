

using System;
using System.Collections.Generic;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Hardware.Camera2;
using Android.Graphics;
using Android.Hardware.Camera2.Params;
using Android.Media;
using Android.Support.V13.App;
using Android.Support.V4.Content;
using Java.IO;
using Java.Lang;
using Java.Nio;
using Java.Util;
using Java.Util.Concurrent;
using Boolean = Java.Lang.Boolean;
using CameraError = Android.Hardware.Camera2.CameraError;
using Math = Java.Lang.Math;
using Object = Java.Lang.Object;
using Orientation = Android.Content.Res.Orientation;

namespace Camera2Basic
{
    public class Camera2BasicFragment : Fragment, View.IOnClickListener, FragmentCompat.IOnRequestPermissionsResultCallback
    {
        private static readonly SparseIntArray ORIENTATIONS = new SparseIntArray();
        private static readonly int REQUEST_CAMERA_PERMISSION = 1;
        private static readonly string FRAGMENT_DIALOG = "dialog";

        // Tag for the {@link Log}.
        private static readonly string TAG = "Camera2BasicFragment";

        // Camera state: Showing camera preview.
        private const int STATE_PREVIEW = 0;

        // Camera state: Waiting for the focus to be locked.
        private const int STATE_WAITING_LOCK = 1;

        // Camera state: Waiting for the exposure to be precapture state.
        private const int STATE_WAITING_PRECAPTURE = 2;

        //Camera state: Waiting for the exposure state to be something other than precapture.
        private const int STATE_WAITING_NON_PRECAPTURE = 3;

        // Camera state: Picture was taken.
        private const int STATE_PICTURE_TAKEN = 4;

        // Max preview width that is guaranteed by Camera2 API
        private static readonly int MAX_PREVIEW_WIDTH = 1920;

        // Max preview height that is guaranteed by Camera2 API
        private static readonly int MAX_PREVIEW_HEIGHT = 1080;

        // TextureView.ISurfaceTextureListener handles several lifecycle events on a TextureView
        private Camera2BasicSurfaceTextureListener mSurfaceTextureListener;
        private class Camera2BasicSurfaceTextureListener : Java.Lang.Object, TextureView.ISurfaceTextureListener
        {
            public Camera2BasicFragment Owner { get; set; }

            public Camera2BasicSurfaceTextureListener(Camera2BasicFragment owner)
            {
                Owner = owner;
            }

            public void OnSurfaceTextureAvailable(Android.Graphics.SurfaceTexture surface, int width, int height)
            {
                Owner.OpenCamera(width, height);
            }

            public bool OnSurfaceTextureDestroyed(Android.Graphics.SurfaceTexture surface)
            {
                return true;
            }

            public void OnSurfaceTextureSizeChanged(Android.Graphics.SurfaceTexture surface, int width, int height)
            {
                Owner.ConfigureTransform(width, height);
            }

            public void OnSurfaceTextureUpdated(Android.Graphics.SurfaceTexture surface)
            {

            }
        }


        // ID of the current {@link CameraDevice}.
        private string mCameraId;

        // An AutoFitTextureView for camera preview
        private AutoFitTextureView mTextureView;

        // A {@link CameraCaptureSession } for camera preview.
        private CameraCaptureSession mCaptureSession;

        // A reference to the opened CameraDevice
        private CameraDevice mCameraDevice;

        // The size of the camera preview
        private Size mPreviewSize;

        // CameraDevice.StateListener is called when a CameraDevice changes its state
        private CameraStateListener mStateCallback;
        private class CameraStateListener : CameraDevice.StateCallback
        {
            public Camera2BasicFragment owner;
            public override void OnOpened(CameraDevice cameraDevice)
            {
                // This method is called when the camera is opened.  We start camera preview here.
                owner.mCameraOpenCloseLock.Release();
                owner.mCameraDevice = cameraDevice;
                owner.CreateCameraPreviewSession();
            }

            public override void OnDisconnected(CameraDevice cameraDevice)
            {
                owner.mCameraOpenCloseLock.Release();
                cameraDevice.Close();
                owner.mCameraDevice = null;
            }

            public override void OnError(CameraDevice cameraDevice, CameraError error)
            {
                owner.mCameraOpenCloseLock.Release();
                cameraDevice.Close();
                owner.mCameraDevice = null;
                if (owner == null)
                    return;
                Activity activity = owner.Activity;
                if (activity != null)
                {
                    activity.Finish();
                }

            }
        }

        // An additional thread for running tasks that shouldn't block the UI.
        private HandlerThread mBackgroundThread;


        // A {@link Handler} for running tasks in the background.
        private Handler mBackgroundHandler;


        // An {@link ImageReader} that handles still image capture.
        private ImageReader mImageReader;

        // This is the output file for our picture.
        private File mFile;

        // This a callback object for the {@link ImageReader}. "onImageAvailable" will be called when a
        // still image is ready to be saved.
        private ImageAvailableListener mOnImageAvailableListener;
        private class ImageAvailableListener : Java.Lang.Object, ImageReader.IOnImageAvailableListener
        {
            public File File { get; set; }
            public Camera2BasicFragment Owner { get; set; }
            public void OnImageAvailable(ImageReader reader)
            {
                Owner.mBackgroundHandler.Post(new ImageSaver(reader.AcquireNextImage(), File));
            }
        }


        //{@link CaptureRequest.Builder} for the camera preview
        private CaptureRequest.Builder mPreviewRequestBuilder;

        // {@link CaptureRequest} generated by {@link #mPreviewRequestBuilder}
        private CaptureRequest mPreviewRequest;

        // The current state of camera state for taking pictures.
        private int mState = STATE_PREVIEW;

        // A {@link Semaphore} to prevent the app from exiting before closing the camera.
        private Semaphore mCameraOpenCloseLock = new Semaphore(1);

        // Whether the current camera device supports Flash or not.
        private bool mFlashSupported;

        // Orientation of the camera sensor
        private int mSensorOrientation;

        // A {@link CameraCaptureSession.CaptureCallback} that handles events related to JPEG capture.
        private CameraCaptureListener mCaptureCallback;
        private class CameraCaptureListener : CameraCaptureSession.CaptureCallback
        {
            public Camera2BasicFragment Owner { get; set; }
            public File File { get; set; }
            public override void OnCaptureCompleted(CameraCaptureSession session, CaptureRequest request, TotalCaptureResult result)
            {
                Process(result);
            }

            public override void OnCaptureProgressed(CameraCaptureSession session, CaptureRequest request, CaptureResult partialResult)
            {
                Process(partialResult);
            }

            private void Process(CaptureResult result)
            {
                switch (Owner.mState)
                {
                    case STATE_WAITING_LOCK:
                        {
                            Integer afState = (Integer)result.Get(CaptureResult.ControlAfState);
                            if (afState == null)
                            {
                                Owner.CaptureStillPicture();
                            }

                            else if ((((int)ControlAFState.FocusedLocked) == afState.IntValue()) ||
                                       (((int)ControlAFState.NotFocusedLocked) == afState.IntValue()))
                            {
                                // ControlAeState can be null on some devices
                                Integer aeState = (Integer)result.Get(CaptureResult.ControlAeState);
                                if (aeState == null ||
                                        aeState.IntValue() == ((int)ControlAEState.Converged))
                                {
                                    Owner.mState = STATE_PICTURE_TAKEN;
                                    Owner.CaptureStillPicture();
                                }
                                else
                                {
                                    Owner.RunPrecaptureSequence();
                                }
                            }
                            break;
                        }
                    case STATE_WAITING_PRECAPTURE:
                        {
                            // ControlAeState can be null on some devices
                            Integer aeState = (Integer)result.Get(CaptureResult.ControlAeState);
                            if (aeState == null ||
                                    aeState.IntValue() == ((int)ControlAEState.Precapture) ||
                                    aeState.IntValue() == ((int)ControlAEState.FlashRequired))
                            {
                                Owner.mState = STATE_WAITING_NON_PRECAPTURE;
                            }
                            break;
                        }
                    case STATE_WAITING_NON_PRECAPTURE:
                        {
                            // ControlAeState can be null on some devices
                            Integer aeState = (Integer)result.Get(CaptureResult.ControlAeState);
                            if (aeState == null || aeState.IntValue() != ((int)ControlAEState.Precapture))
                            {
                                Owner.mState = STATE_PICTURE_TAKEN;
                                Owner.CaptureStillPicture();
                            }
                            break;
                        }
                }
            }
        }

        // Shows a {@link Toast} on the UI thread.
        private void ShowToast(string text)
        {
            if (Activity != null)
            {
                Activity.RunOnUiThread(new ShowToastRunnable(Activity.ApplicationContext, text));
            }
        }

        private class ShowToastRunnable : Java.Lang.Object, IRunnable
        {
            private string text;
            private Context context;

            public ShowToastRunnable(Context context, string text)
            {
                this.context = context;
                this.text = text;
            }

            public void Run()
            {
                Toast.MakeText(context, text, ToastLength.Short).Show();
            }
        }

        private static Size ChooseOptimalSize(Size[] choices, int textureViewWidth,
            int textureViewHeight, int maxWidth, int maxHeight, Size aspectRatio)
        {
            // Collect the supported resolutions that are at least as big as the preview Surface
            var bigEnough = new List<Size>();
            // Collect the supported resolutions that are smaller than the preview Surface
            var notBigEnough = new List<Size>();
            int w = aspectRatio.Width;
            int h = aspectRatio.Height;

            for (var i = 0; i < choices.Length; i++)
            {
                Size option = choices[i];
                if ((option.Width <= maxWidth) && (option.Height <= maxHeight) &&
                       option.Height == option.Width * h / w)
                {
                    if (option.Width >= textureViewWidth &&
                        option.Height >= textureViewHeight)
                    {
                        bigEnough.Add(option);
                    }
                    else
                    {
                        notBigEnough.Add(option);
                    }
                }
            }

            // Pick the smallest of those big enough. If there is no one big enough, pick the
            // largest of those not big enough.
            if (bigEnough.Count > 0)
            {
                return (Size) Collections.Min(bigEnough, new CompareSizesByArea());
            }
            else if (notBigEnough.Count > 0)
            {
                return (Size) Collections.Max(notBigEnough, new CompareSizesByArea());
            }
            else
            {
                Log.Error(TAG, "Couldn't find any suitable preview size");
                return choices[0];
            }
        }

        public static Camera2BasicFragment NewInstance()
        {
            return new Camera2BasicFragment();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mStateCallback = new CameraStateListener() { owner = this };
            mSurfaceTextureListener = new Camera2BasicSurfaceTextureListener(this);

            // fill ORIENTATIONS list
            ORIENTATIONS.Append((int)SurfaceOrientation.Rotation0, 90);
            ORIENTATIONS.Append((int)SurfaceOrientation.Rotation90, 0);
            ORIENTATIONS.Append((int)SurfaceOrientation.Rotation180, 270);
            ORIENTATIONS.Append((int)SurfaceOrientation.Rotation270, 180);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_camera2_basic, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            mTextureView = (AutoFitTextureView)view.FindViewById(Resource.Id.texture);
            view.FindViewById(Resource.Id.picture).SetOnClickListener(this);
            view.FindViewById(Resource.Id.info).SetOnClickListener(this);
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            mFile = new File(Activity.GetExternalFilesDir(null), "pic.jpg");
        }

        public override void OnResume()
        {
            base.OnResume();
            StartBackgroundThread();

            // When the screen is turned off and turned back on, the SurfaceTexture is already
            // available, and "onSurfaceTextureAvailable" will not be called. In that case, we can open
            // a camera and start preview from here (otherwise, we wait until the surface is ready in
            // the SurfaceTextureListener).
            if (mTextureView.IsAvailable)
            {
                OpenCamera(mTextureView.Width, mTextureView.Height);
            }
            else
            {
                mTextureView.SurfaceTextureListener = mSurfaceTextureListener;
            }
        }

        public override void OnPause()
        {
            CloseCamera();
            StopBackgroundThread();
            base.OnPause();
        }

        private void RequestCameraPermission()
        {
            if (FragmentCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.Camera))
            {
                new ConfirmationDialog().Show(ChildFragmentManager, FRAGMENT_DIALOG);
            }
            else
            {
                FragmentCompat.RequestPermissions(this, new string[] { Manifest.Permission.Camera },
                        REQUEST_CAMERA_PERMISSION);
            }
        }

        public void OnRequestPermissionsResult(int requestCode, string[] permissions, int[] grantResults)
        {
            if (requestCode != REQUEST_CAMERA_PERMISSION)
                return;

            if (grantResults.Length != 1 || grantResults[0] != (int)Permission.Granted)
            {
                ErrorDialog.NewInstance(GetString(Resource.String.request_permission))
                        .Show(ChildFragmentManager, FRAGMENT_DIALOG);
            }
        }


        // Sets up member variables related to camera.
        private void SetUpCameraOutputs(int width, int height)
        {
            var activity = Activity;
            var manager = (CameraManager)activity.GetSystemService(Context.CameraService);
            try
            {
                for (var i = 0; i < manager.GetCameraIdList().Length; i++)
                {
                    var cameraId = manager.GetCameraIdList()[i];
                    CameraCharacteristics characteristics = manager.GetCameraCharacteristics(cameraId);

                    // We don't use a front facing camera in this sample.
                    var facing = (Integer)characteristics.Get(CameraCharacteristics.LensFacing);
                    if (facing != null && facing == (Integer.ValueOf((int)LensFacing.Front)))
                    {
                        continue;
                    }

                    var map = (StreamConfigurationMap)characteristics.Get(CameraCharacteristics.ScalerStreamConfigurationMap);
                    if (map == null)
                    {
                        continue;
                    }

                    // For still image captures, we use the largest available size.
                    Size largest = (Size) Collections.Max(Arrays.AsList(map.GetOutputSizes((int)ImageFormatType.Jpeg)),
                        new CompareSizesByArea());
                    mImageReader = ImageReader.NewInstance(largest.Width, largest.Height, ImageFormatType.Jpeg, /*maxImages*/2);
                    mImageReader.SetOnImageAvailableListener(mOnImageAvailableListener, mBackgroundHandler);

                    // Find out if we need to swap dimension to get the preview size relative to sensor
                    // coordinate.
                    var displayRotation = activity.WindowManager.DefaultDisplay.Rotation;
                    //noinspection ConstantConditions
                    mSensorOrientation = (int)characteristics.Get(CameraCharacteristics.SensorOrientation);
                    bool swappedDimensions = false;
                    switch (displayRotation)
                    {
                        case SurfaceOrientation.Rotation0:
                        case SurfaceOrientation.Rotation180:
                            if (mSensorOrientation == 90 || mSensorOrientation == 270)
                            {
                                swappedDimensions = true;
                            }
                            break;
                        case SurfaceOrientation.Rotation90:
                        case SurfaceOrientation.Rotation270:
                            if (mSensorOrientation == 0 || mSensorOrientation == 180)
                            {
                                swappedDimensions = true;
                            }
                            break;
                        default:
                            Log.Error(TAG, "Display rotation is invalid: " + displayRotation);
                            break;
                    }

                    Point displaySize = new Point();
                    activity.WindowManager.DefaultDisplay.GetSize(displaySize);
                    var rotatedPreviewWidth = width;
                    var rotatedPreviewHeight = height;
                    var maxPreviewWidth = displaySize.X;
                    var maxPreviewHeight = displaySize.Y;

                    if (swappedDimensions)
                    {
                        rotatedPreviewWidth = height;
                        rotatedPreviewHeight = width;
                        maxPreviewWidth = displaySize.Y;
                        maxPreviewHeight = displaySize.X;
                    }

                    if (maxPreviewWidth > MAX_PREVIEW_WIDTH)
                    {
                        maxPreviewWidth = MAX_PREVIEW_WIDTH;
                    }

                    if (maxPreviewHeight > MAX_PREVIEW_HEIGHT)
                    {
                        maxPreviewHeight = MAX_PREVIEW_HEIGHT;
                    }

                    // Danger, W.R.! Attempting to use too large a preview size could  exceed the camera
                    // bus' bandwidth limitation, resulting in gorgeous previews but the storage of
                    // garbage capture data.
                    mPreviewSize = ChooseOptimalSize(map.GetOutputSizes(Class.FromType(typeof(SurfaceTexture))),
                        rotatedPreviewWidth, rotatedPreviewHeight, maxPreviewWidth,
                        maxPreviewHeight, largest);

                    // We fit the aspect ratio of TextureView to the size of preview we picked.
                    var orientation = Resources.Configuration.Orientation;
                    if (orientation == Orientation.Landscape)
                    {
                        mTextureView.SetAspectRatio(mPreviewSize.Width, mPreviewSize.Height);
                    }
                    else
                    {
                        mTextureView.SetAspectRatio(mPreviewSize.Height, mPreviewSize.Width);
                    }

                    // Check if the flash is supported.
                    var available = (Boolean)characteristics.Get(CameraCharacteristics.FlashInfoAvailable);
                    if (available == null)
                    {
                        mFlashSupported = false;
                    }
                    else
                    {
                        mFlashSupported = (bool)available;
                    }

                    mCameraId = cameraId;
                    return;
                }
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
            catch (NullPointerException e)
            {
                // Currently an NPE is thrown when the Camera2API is used but not supported on the
                // device this code runs.
                ErrorDialog.NewInstance(GetString(Resource.String.camera_error)).Show(ChildFragmentManager, FRAGMENT_DIALOG);
            }
        }

        // Opens the camera specified by {@link Camera2BasicFragment#mCameraId}.
        private void OpenCamera(int width, int height)
        {
            if (ContextCompat.CheckSelfPermission(Activity, Manifest.Permission.Camera) != Permission.Granted)
            {
                RequestCameraPermission();
                return;
            }
            SetUpCameraOutputs(width, height);
            ConfigureTransform(width, height);
            var activity = Activity;
            var manager = (CameraManager)activity.GetSystemService(Context.CameraService);
            try
            {
                if (!mCameraOpenCloseLock.TryAcquire(2500, TimeUnit.Microseconds))
                {
                    throw new RuntimeException("Time out waiting to lock camera opening.");
                }
                manager.OpenCamera(mCameraId, mStateCallback, mBackgroundHandler);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
            catch (InterruptedException e)
            {
                throw new RuntimeException("Interrupted while trying to lock camera opening.", e);
            }
        }

        // Closes the current {@link CameraDevice}.
        private void CloseCamera()
        {
            try
            {
                mCameraOpenCloseLock.Acquire();
                if (null != mCaptureSession)
                {
                    mCaptureSession.Close();
                    mCaptureSession = null;
                }
                if (null != mCameraDevice)
                {
                    mCameraDevice.Close();
                    mCameraDevice = null;
                }
                if (null != mImageReader)
                {
                    mImageReader.Close();
                    mImageReader = null;
                }
            }
            catch (InterruptedException e)
            {
                throw new RuntimeException("Interrupted while trying to lock camera closing.", e);
            }
            finally
            {
                mCameraOpenCloseLock.Release();
            }
        }

        // Starts a background thread and its {@link Handler}.
        private void StartBackgroundThread()
        {
            mBackgroundThread = new HandlerThread("CameraBackground");
            mBackgroundThread.Start();
            mBackgroundHandler = new Handler(mBackgroundThread.Looper);
        }

        // Stops the background thread and its {@link Handler}.
        private void StopBackgroundThread()
        {
            mBackgroundThread.QuitSafely();
            try
            {
                mBackgroundThread.Join();
                mBackgroundThread = null;
                mBackgroundHandler = null;
            }
            catch (InterruptedException e)
            {
                e.PrintStackTrace();
            }
        }

        // Creates a new {@link CameraCaptureSession} for camera preview.
        private void CreateCameraPreviewSession()
        {
            try
            {
                SurfaceTexture texture = mTextureView.SurfaceTexture;
                if (texture == null)
                {
                    throw new IllegalStateException("texture is null");
                }

                // We configure the size of default buffer to be the size of camera preview we want.
                texture.SetDefaultBufferSize(mPreviewSize.Width, mPreviewSize.Height);

                // This is the output Surface we need to start preview.
                Surface surface = new Surface(texture);

                // We set up a CaptureRequest.Builder with the output Surface.
                mPreviewRequestBuilder = mCameraDevice.CreateCaptureRequest(CameraTemplate.Preview);
                mPreviewRequestBuilder.AddTarget(surface);

                // Here, we create a CameraCaptureSession for camera preview.
                List<Surface> surfaces = new List<Surface>();
                surfaces.Add(surface);
                surfaces.Add(mImageReader.Surface);
                mCameraDevice.CreateCaptureSession(surfaces, new CameraCaptureSessionCallback(this), null);

            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }

        private class CameraCaptureSessionCallback : CameraCaptureSession.StateCallback
        {

            private Camera2BasicFragment owner;

            public CameraCaptureSessionCallback(Camera2BasicFragment owner)
            {
                this.owner = owner;
            }

            public override void OnConfigureFailed(CameraCaptureSession session)
            {
                owner.ShowToast("Failed");
            }

            public override void OnConfigured(CameraCaptureSession session)
            {
                // The camera is already closed
                if (null == owner.mCameraDevice)
                {
                    return;
                }

                // When the session is ready, we start displaying the preview.
                owner.mCaptureSession = session;
                try
                {
                    // Auto focus should be continuous for camera preview.
                    owner.mPreviewRequestBuilder.Set(CaptureRequest.ControlAfMode, (int)ControlAFMode.ContinuousPicture);
                    // Flash is automatically enabled when necessary.
                    owner.SetAutoFlash(owner.mPreviewRequestBuilder);

                    // Finally, we start displaying the camera preview.
                    owner.mPreviewRequest = owner.mPreviewRequestBuilder.Build();
                    owner.mCaptureSession.SetRepeatingRequest(owner.mPreviewRequest,
                            owner.mCaptureCallback, owner.mBackgroundHandler);
                }
                catch (CameraAccessException e)
                {
                    e.PrintStackTrace();
                }
            }
        }

        public static T Cast<T>(Java.Lang.Object obj) where T : class
        {
            var propertyInfo = obj.GetType().GetProperty("Instance");
            return propertyInfo == null ? null : propertyInfo.GetValue(obj, null) as T;
        }

        // Configures the necessary {@link android.graphics.Matrix}
        // transformation to `mTextureView`.
        // This method should be called after the camera preview size is determined in
        // setUpCameraOutputs and also the size of `mTextureView` is fixed.

        private void ConfigureTransform(int viewWidth, int viewHeight)
        {
            Activity activity = Activity;
            if (null == mTextureView || null == mPreviewSize || null == activity)
            {
                return;
            }
            var rotation = (int)activity.WindowManager.DefaultDisplay.Rotation;
            Matrix matrix = new Matrix();
            RectF viewRect = new RectF(0, 0, viewWidth, viewHeight);
            RectF bufferRect = new RectF(0, 0, mPreviewSize.Height, mPreviewSize.Width);
            float centerX = viewRect.CenterX();
            float centerY = viewRect.CenterY();
            if ((int)SurfaceOrientation.Rotation90 == rotation || (int)SurfaceOrientation.Rotation270 == rotation)
            {
                bufferRect.Offset(centerX - bufferRect.CenterX(), centerY - bufferRect.CenterY());
                matrix.SetRectToRect(viewRect, bufferRect, Matrix.ScaleToFit.Fill);
                float scale = Math.Max((float)viewHeight / mPreviewSize.Height, (float)viewWidth / mPreviewSize.Width);
                matrix.PostScale(scale, scale, centerX, centerY);
                matrix.PostRotate(90 * (rotation - 2), centerX, centerY);
            }
            else if ((int)SurfaceOrientation.Rotation180 == rotation)
            {
                matrix.PostRotate(180, centerX, centerY);
            }
            mTextureView.SetTransform(matrix);
        }

        // Initiate a still image capture.
        private void TakePicture()
        {
            LockFocus();
        }

        // Lock the focus as the first step for a still image capture.
        private void LockFocus()
        {
            try
            {
                // This is how to tell the camera to lock focus.

                mPreviewRequestBuilder.Set(CaptureRequest.ControlAfTrigger, (int)ControlAFTrigger.Start);
                // Tell #mCaptureCallback to wait for the lock.
                mState = STATE_WAITING_LOCK;
                mCaptureSession.Capture(mPreviewRequestBuilder.Build(), mCaptureCallback,
                        mBackgroundHandler);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }

        // Run the precapture sequence for capturing a still image. This method should be called when
        // we get a response in {@link #mCaptureCallback} from {@link #lockFocus()}.
        private void RunPrecaptureSequence()
        {
            try
            {
                // This is how to tell the camera to trigger.
                mPreviewRequestBuilder.Set(CaptureRequest.ControlAePrecaptureTrigger, (int)ControlAEPrecaptureTrigger.Start);
                // Tell #mCaptureCallback to wait for the precapture sequence to be set.
                mState = STATE_WAITING_PRECAPTURE;
                mCaptureSession.Capture(mPreviewRequestBuilder.Build(), mCaptureCallback, mBackgroundHandler);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }

        // Capture a still picture. This method should be called when we get a response in
        // {@link #mCaptureCallback} from both {@link #lockFocus()}.
        private void CaptureStillPicture()
        {
            try
            {
                var activity = Activity;
                if (null == activity || null == mCameraDevice)
                {
                    return;
                }
                // This is the CaptureRequest.Builder that we use to take a picture.
                CaptureRequest.Builder captureBuilder = mCameraDevice.CreateCaptureRequest(CameraTemplate.StillCapture);
                captureBuilder.AddTarget(mImageReader.Surface);

                // Use the same AE and AF modes as the preview.
                captureBuilder.Set(CaptureRequest.ControlAfMode, (int)ControlAFMode.ContinuousPicture);
                SetAutoFlash(captureBuilder);

                // Orientation
                int rotation = (int)activity.WindowManager.DefaultDisplay.Rotation;
                captureBuilder.Set(CaptureRequest.JpegOrientation, GetOrientation(rotation));

                mCaptureSession.StopRepeating();
                mCaptureSession.Capture(captureBuilder.Build(), new CameraCaptureStillPictureSessionCallback(this), null);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }

        private class CameraCaptureStillPictureSessionCallback : CameraCaptureSession.CaptureCallback
        {
            private Camera2BasicFragment owner;

            public CameraCaptureStillPictureSessionCallback(Camera2BasicFragment owner)
            {
                this.owner = owner;
            }

            public override void OnCaptureCompleted(CameraCaptureSession session, CaptureRequest request, TotalCaptureResult result)
            {
                owner.ShowToast("Saved: " + owner.mFile);
                Log.Debug(TAG, owner.mFile.ToString());
                owner.UnlockFocus();
            }
        }

        // Retrieves the JPEG orientation from the specified screen rotation.
        private int GetOrientation(int rotation)
        {
            // Sensor orientation is 90 for most devices, or 270 for some devices (eg. Nexus 5X)
            // We have to take that into account and rotate JPEG properly.
            // For devices with orientation of 90, we simply return our mapping from ORIENTATIONS.
            // For devices with orientation of 270, we need to rotate the JPEG 180 degrees.
            return (ORIENTATIONS.Get(rotation) + mSensorOrientation + 270) % 360;
        }

        // Unlock the focus. This method should be called when still image capture sequence is
        // finished.
        private void UnlockFocus()
        {
            try
            {
                // Reset the auto-focus trigger
                mPreviewRequestBuilder.Set(CaptureRequest.ControlAfTrigger, (int)ControlAFTrigger.Cancel);
                SetAutoFlash(mPreviewRequestBuilder);
                mCaptureSession.Capture(mPreviewRequestBuilder.Build(), mCaptureCallback,
                        mBackgroundHandler);
                // After this, the camera will go back to the normal state of preview.
                mState = STATE_PREVIEW;
                mCaptureSession.SetRepeatingRequest(mPreviewRequest, mCaptureCallback,
                        mBackgroundHandler);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.picture:
                    TakePicture();
                    break;
                case Resource.Id.info:
                    EventHandler<DialogClickEventArgs> nullHandler = null;
                    Activity activity = Activity;
                    if (activity != null)
                    {
                        new AlertDialog.Builder(activity)
                            .SetMessage("This sample demonstrates the basic use of the Camera2 API. ...")
                            .SetPositiveButton(Android.Resource.String.Ok, nullHandler)
                            .Show();
                    }
                    break;
            }
        }

        private void SetAutoFlash(CaptureRequest.Builder requestBuilder)
        {
            if (mFlashSupported)
            {
                requestBuilder.Set(CaptureRequest.ControlAeMode, (int)ControlAEMode.OnAutoFlash);
            }
        }

        // Saves a JPEG {@link Image} into the specified {@link File}.
        private class ImageSaver : Java.Lang.Object, IRunnable
        {
            // The JPEG image
            private Image mImage;

            // The file we save the image into.
            private File mFile;

            public ImageSaver(Image image, File file)
            {
                mImage = image;
                mFile = file;
            }

            public void Run()
            {
                ByteBuffer buffer = mImage.GetPlanes()[0].Buffer;
                byte[] bytes = new byte[buffer.Remaining()];
                buffer.Get(bytes);
                using (var output = new FileOutputStream(mFile))
                {
                    try
                    {
                        output.Write(bytes);
                    }
                    catch (IOException e)
                    {
                        e.PrintStackTrace();
                    }
                    finally
                    {
                        mImage.Close();
                    }
                }
            }
        }

        // Compares two {@code Size}s based on their areas.
        public class CompareSizesByArea : Java.Lang.Object, IComparator
        {
            public int Compare(Object lhs, Object rhs)
            {
                var lhsSize = (Size)lhs;
                var rhsSize = (Size)rhs;
                // We cast here to ensure the multiplications won't overflow
                return Long.Signum((long)lhsSize.Width * lhsSize.Height - (long)rhsSize.Width * rhsSize.Height);
            }
        }

        // Shows an error message dialog.
        public class ErrorDialog : DialogFragment
        {
            private static readonly string ARG_MESSAGE = "message";
            private static Activity mActivity;

            private class PositiveListener : Java.Lang.Object, IDialogInterfaceOnClickListener
            {
                public void OnClick(IDialogInterface dialog, int which)
                {
                    mActivity.Finish();
                }
            }

            public static ErrorDialog NewInstance(string message)
            {
                var args = new Bundle();
                args.PutString(ARG_MESSAGE, message);
                return new ErrorDialog { Arguments = args };
            }

            public override Dialog OnCreateDialog(Bundle savedInstanceState)
            {
                mActivity = Activity;
                return new AlertDialog.Builder(mActivity)
                    .SetMessage(Arguments.GetString(ARG_MESSAGE))
                    .SetPositiveButton(Android.Resource.String.Ok, new PositiveListener())
                    .Create();
            }
        }

        // Shows OK/Cancel confirmation dialog about camera permission.
        public class ConfirmationDialog : DialogFragment
        {
            private static Fragment mParent;
            private class PositiveListener : Java.Lang.Object, IDialogInterfaceOnClickListener
            {
                public void OnClick(IDialogInterface dialog, int which)
                {
                    FragmentCompat.RequestPermissions(mParent,
                                    new string[] { Manifest.Permission.Camera }, REQUEST_CAMERA_PERMISSION);
                }
            }

            private class NegativeListener : Java.Lang.Object, IDialogInterfaceOnClickListener
            {
                public void OnClick(IDialogInterface dialog, int which)
                {
                    Activity activity = mParent.Activity;
                    if (activity != null)
                    {
                        activity.Finish();
                    }
                }
            }

            public override Dialog OnCreateDialog(Bundle savedInstanceState)
            {
                mParent = ParentFragment;
                return new AlertDialog.Builder(Activity)
                    .SetMessage(Resource.String.request_permission)
                    .SetPositiveButton(Android.Resource.String.Ok, new PositiveListener())
                    .SetNegativeButton(Android.Resource.String.Cancel, new NegativeListener())
                    .Create();
            }
        }

    }
}

