using System;
using Android.OS;
using Android.App;

namespace TvLeanback
{
	[Activity (Icon="@drawable/videos_by_google_banner", Label="@string/app_name", 
		ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape, MainLauncher = true)]
	[IntentFilter(
		actions : new string[] { "android.intent.action.MAIN" },
		Categories = new string[] { "android.intent.category.LEANBACK_LAUNCHER" })]
	public class MainActivity : Activity
	{
		protected override void OnCreate(Bundle saved)
		{
			base.OnCreate (saved);
			SetContentView (Resource.Layout.main);
		}
	}
}
