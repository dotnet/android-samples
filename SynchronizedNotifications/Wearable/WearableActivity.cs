using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace Wearable
{
	[Activity (Label = "@string/app_name", MainLauncher = true)]
	public class WearableActivity : Activity 
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.activity_wearable);
		}
	}
}


