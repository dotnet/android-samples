using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Telephony;
using Android.Widget;

namespace GetMobileNetworkStrength
{
	[Activity(Label = "GetMobileNetworkStrength", MainLauncher = true)]
	public class MainActivity : Activity
	{
		Button _getGsmSignalStrengthButton;
		TelephonyManager _telephonyManager;
		GsmSignalStrengthListener _signalStrengthListener;
		ImageView _gmsStrengthImageView;
		TextView _gmsStrengthTextView;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Main);

			// Get a reference to the TelephonyManager and instantiate the GsmSignalStrengthListener.
			// These will be used by the Button's OnClick event handler.
			_telephonyManager = (TelephonyManager)GetSystemService(Context.TelephonyService);
			_signalStrengthListener = new GsmSignalStrengthListener();

			_gmsStrengthTextView = FindViewById<TextView>(Resource.Id.textView1);
			_gmsStrengthImageView = FindViewById<ImageView>(Resource.Id.imageView1);
			_getGsmSignalStrengthButton = FindViewById<Button>(Resource.Id.myButton);

			_getGsmSignalStrengthButton.Click += DisplaySignalStrength;
		}

		void DisplaySignalStrength(object sender, EventArgs e)
		{
			_telephonyManager.Listen(_signalStrengthListener, PhoneStateListenerFlags.SignalStrengths);
			_signalStrengthListener.SignalStrengthChanged += HandleSignalStrengthChanged;
		}

		void HandleSignalStrengthChanged(int strength)
		{
			// We want this to be a one-shot thing when the button is pushed. Make sure to unhook everything
			_signalStrengthListener.SignalStrengthChanged -= HandleSignalStrengthChanged;
			_telephonyManager.Listen(_signalStrengthListener, PhoneStateListenerFlags.None);

			// Update the UI with text and an image.
			_gmsStrengthImageView.SetImageLevel(strength);
			_gmsStrengthTextView.Text = string.Format("GPS Signal Strength ({0}):", strength);
		}
	}
}


