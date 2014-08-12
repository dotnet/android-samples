using System;
using System.Collections;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7;
using Android.Annotation;
using Android.Hardware.Input;
using Android.Util;
using VisualGameController;

using Java.Lang;

namespace VisualGameController
{
	//A group of small classes used in the control of the navigation
	public class MyOnClickListener: Java.Lang.Object,View.IOnClickListener
	{
		FullScreenActivity activity;
		public MyOnClickListener(FullScreenActivity activity)
		{
			this.activity = activity;
		}

		public void OnClick(View View)
		{
			if (FullScreenActivity.TOGGLE_ON_CLICK) {
				activity.system_ui_hider.Toggle();
			} else {
				activity.system_ui_hider.Show();
			}
		}
	}

	public class MyOnTouchListener : Java.Lang.Object,View.IOnTouchListener
	{
		FullScreenActivity activity;
		public MyOnTouchListener(FullScreenActivity activity)
		{
			this.activity = activity;
		}
		public bool OnTouch(View view, MotionEvent motion_event)
		{
			if (FullScreenActivity.AUTO_HIDE) 
				activity.DelayedHide(FullScreenActivity.AUTO_HIDE_DELAY_MILLIS);

			return false;
		}
	}

	public class MyRunnable : Java.Lang.Object,IRunnable
	{
		FullScreenActivity activity;
		public MyRunnable(FullScreenActivity activity)
		{
			this.activity = activity;
		}
		public void Run()
		{
			activity.system_ui_hider.Hide();
		}
	}

	public class MyOnVisibilityChangeListener: SystemUiHider.OnVisibilityChangeListner 
	{
		FullScreenActivity activity;
		int controls_height;
		int short_anim_time;
		public MyOnVisibilityChangeListener(FullScreenActivity activity)
		{
			this.activity = activity;
		}
		public void OnVisibilityChange(bool visible)
		{
			if (Build.VERSION.SdkInt >= BuildVersionCodes.Honeycomb) {
				if (controls_height == 0) 
					controls_height = activity.controlsView.Height;

				if (short_anim_time == 0) 
					short_anim_time = activity.Resources.GetInteger (Android.Resource.Integer.ConfigShortAnimTime);

				activity.controlsView.Animate ().TranslationY (visible ? 0 : controls_height).SetDuration (short_anim_time);
			} else {
				activity.controlsView.Visibility = (visible ? ViewStates.Visible : ViewStates.Gone);
			}

			if (visible && FullScreenActivity.AUTO_HIDE) 
				activity.DelayedHide (FullScreenActivity.AUTO_HIDE_DELAY_MILLIS);
		}
	}


	[Activity (Label = "Visual-Game-Controller", MainLauncher = true, Icon = "@drawable/icon")]
	public class FullScreenActivity : Activity,InputManager.IInputDeviceListener
	{
		private const string TAG = "FullscreenActivity";

		public const bool AUTO_HIDE = true;

		public const int AUTO_HIDE_DELAY_MILLIS=3000;

		public const bool TOGGLE_ON_CLICK=true;

		private const int HIDER_FLAGS = SystemUiHider.FLAG_HIDE_NAVIGATION;

		private ControllerView controller_view;

		public View controlsView;
		public View contentView;

		public SystemUiHider system_ui_hider;

		private int[] buttons = new int[ButtonMapping.size];
		private float[] axes = new float[AxesMapping.size];
		private InputManager input_manager;
		private List<int> connected_devices = new List<int> ();
		private int current_device_id=-1;
		private Handler hide_handler;
		private MyOnTouchListener delay_hide_touch_listener;
		private MyRunnable hide_runnable;

		public FullScreenActivity()
		{
			delay_hide_touch_listener = new MyOnTouchListener (this);
			hide_handler = new Handler();
			hide_runnable = new MyRunnable(this);
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			//Set the window fullscreen so that the background is drawn the correct size
			Window.AddFlags(WindowManagerFlags.KeepScreenOn);
			Window.AddFlags (WindowManagerFlags.Fullscreen);
			Window.AddFlags (WindowManagerFlags.LayoutNoLimits);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.activity_fullscreen);

			controlsView = FindViewById (Resource.Id.fullscreen_content_controls);
			contentView = FindViewById (Resource.Id.fullscreen_content);

			controller_view = (ControllerView)FindViewById (Resource.Id.controller);

			for (int i = 0; i < buttons.Length; i++) 
				buttons [i] = 0;
			for (int i = 0; i < axes.Length; i++) 
				axes [i] = 0.0f;

			controller_view.SetButtonAxes (buttons, axes);

			//Set up a UI hider to control the UI visibility for the activity
			system_ui_hider = SystemUiHider.GetInstance (this, contentView, HIDER_FLAGS);
			system_ui_hider.Setup ();
			system_ui_hider.SetOnVisibilityChangeListener (new MyOnVisibilityChangeListener (this));

			contentView.SetOnClickListener (new MyOnClickListener (this));
			input_manager = (InputManager)GetSystemService (Context.InputService);
			CheckGameControllers ();

		}

		//Check for any connected game controllers
		private void CheckGameControllers()
		{
			Log.Debug (TAG, "CheckGameControllers");
			int[] deviceIds = input_manager.GetInputDeviceIds ();
			foreach (int deviceId in deviceIds) {
				InputDevice dev = InputDevice.GetDevice (deviceId);
				int sources = (int)dev.Sources;

				if (((sources & (int)InputSourceType.Gamepad) == (int)InputSourceType.Gamepad) ||
					((sources & (int)InputSourceType.Joystick) == (int)InputSourceType.Joystick)) {
					if (!connected_devices.Contains (deviceId)) {
						connected_devices.Add (deviceId);
						if (current_device_id == -1) {
							current_device_id = deviceId;
							controller_view.SetCurrentControllerNumber (dev.ControllerNumber);
							controller_view.Invalidate ();
						}
					}
				}
			}
		}

		protected override void OnPostCreate (Bundle savedInstanceState)
		{
			base.OnPostCreate (savedInstanceState);
			DelayedHide(100);
		}

		protected override void OnResume ()
		{
			base.OnResume ();
			input_manager.RegisterInputDeviceListener (this, null);
		}

		protected override void OnPause ()
		{
			base.OnPause ();
			input_manager.UnregisterInputDeviceListener (this);
		}

		public void DelayedHide(int delay_millis)
		{
			hide_handler.RemoveCallbacks (hide_runnable);
			hide_handler.PostDelayed (hide_runnable, delay_millis);
		}

		public override bool OnTouchEvent (MotionEvent e)
		{
			return base.OnTouchEvent (e);
		}

		public override bool OnGenericMotionEvent (MotionEvent e)
		{
			InputDevice device = e.Device;
			if (device != null && device.Id == current_device_id) {
				if (IsGamepad (device)) {
					for(int i = 0;i< AxesMapping.size;i++){
						axes [i] = GetCenteredAxis (e, device, AxesMapping.OrdinalValueAxis(i));
					}
					controller_view.Invalidate ();
					return true;
				}
			}
			return base.OnGenericMotionEvent (e);
		}

		//Get the centered position for the joystick axis
		private float GetCenteredAxis(MotionEvent e, InputDevice device, Axis axis)
		{
			InputDevice.MotionRange range = device.GetMotionRange (axis, e.Source);
			if (range != null) {
				float flat = range.Flat;
				float value = e.GetAxisValue (axis);
				if (System.Math.Abs (value) > flat) 
					return value;
			}

			return 0;

		}
			
		public override bool OnKeyDown (Keycode keyCode, KeyEvent e)
		{
			InputDevice device = e.Device;
			if (device != null && device.Id == current_device_id) {
				if (IsGamepad (device)) {
					int index = ButtonMapping.OrdinalValue (keyCode);
					if (index >= 0) {
						buttons [index] = 1;
						controller_view.Invalidate ();
					}
					return true;
				}
			}
			return base.OnKeyDown (keyCode, e);
		}

		public override bool OnKeyUp (Keycode keyCode, KeyEvent e)
		{
			InputDevice device = e.Device;
			if (device != null && device.Id == current_device_id) {
				int index = ButtonMapping.OrdinalValue (keyCode);
				if (index >= 0) {
					buttons [index] = 0;
					controller_view.Invalidate ();
				}
				return true;
			}
			return base.OnKeyUp (keyCode, e);
		}

		private bool IsGamepad(InputDevice device)
		{
			if ((device.Sources & InputSourceType.Gamepad) == InputSourceType.Gamepad ||
			   (device.Sources & InputSourceType.ClassJoystick) == InputSourceType.Joystick) {
				return true;
			}
			return false;
		}

		public void OnInputDeviceAdded(int deviceId)
		{
			Log.Debug (TAG, "OnInputDeviceAdded: " + deviceId);
			if (!connected_devices.Contains (deviceId)) {
				connected_devices.Add(deviceId);
			}
			if (current_device_id == -1) {
				current_device_id = deviceId;
				InputDevice dev = InputDevice.GetDevice (current_device_id);
				if (dev != null) {
					controller_view.SetCurrentControllerNumber (dev.ControllerNumber);
					controller_view.Invalidate ();
				}
			}
		}

		public void OnInputDeviceRemoved(int deviceId)
		{
			Log.Debug (TAG, "OnInputDeviceRemoved: ", deviceId);
			connected_devices.Remove (deviceId);
			if (current_device_id == deviceId) 
				current_device_id = -1;

			if (connected_devices.Count == 0) {
				controller_view.SetCurrentControllerNumber (-1);
				controller_view.Invalidate ();
			} else {
				current_device_id = connected_devices [0];
				InputDevice dev = InputDevice.GetDevice (current_device_id);
				if (dev != null) {
					controller_view.SetCurrentControllerNumber (dev.ControllerNumber);
					controller_view.Invalidate ();
				}
			}

		}

		public void OnInputDeviceChanged(int deviceId)
		{
			Log.Debug (TAG, "OnInputDeviceChanged: " + deviceId);
			controller_view.Invalidate ();
		}
			
	}
}


