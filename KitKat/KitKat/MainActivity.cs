using System;

using Android.App;
using Android.OS;
using Android.Widget;



namespace KitKat
{
	[Activity (Label = "KitKat", MainLauncher = true)]
	public class MainActivity : Activity
	{

		Button uiButton;
		Button safButton;
		Button printButton;
		Button sensorButton;

		#region Activity Lifecycle

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Main);

			uiButton = FindViewById<Button> (Resource.Id.uiButton);
			safButton = FindViewById<Button> (Resource.Id.safButton);
			printButton = FindViewById<Button> (Resource.Id.printButton);
			sensorButton = FindViewById<Button> (Resource.Id.sensorButton);

			uiButton.Click += (o, e) => {
				StartActivity (typeof(TransitionActivity));
			};

			safButton.Click += (o, e) => {
				StartActivity (typeof(StorageActivity));
			};

			printButton.Click += (o, e) => {
				StartActivity (typeof(PrintHtmlActivity));
			};

			sensorButton.Click += (o, e) => {
				StartActivity (typeof(SensorsActivity));
			};

		}
			
		#endregion

	}
}

