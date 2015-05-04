using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content.PM;

namespace WalkingGame
{
	[Activity(Label = "MonoGame"
		, MainLauncher = true
		, Icon = "@drawable/icon"
		, AlwaysRetainTaskState = true
		, LaunchMode = LaunchMode.SingleInstance
		, ScreenOrientation = ScreenOrientation.SensorLandscape
		, ConfigurationChanges = ConfigChanges.Orientation | 
		ConfigChanges.Keyboard | 
		ConfigChanges.KeyboardHidden)]
	public class MainActivity : Microsoft.Xna.Framework.AndroidGameActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate (bundle);
			var g = new MainGame ();
			SetContentView((View)g.Services.GetService(typeof(View)));
			g.Run ();
		}
	}
}


