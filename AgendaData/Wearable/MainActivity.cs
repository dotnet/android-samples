
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using Android.Gms.Wearable;

namespace Wearable
{
	[Activity (Label = "MainActivity", MainLauncher = true)]			
	public class MainActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			if (Log.IsLoggable (Constants.TAG, LogPriority.Info))
				Log.Info (Constants.TAG, "Main activity launched");
			SetContentView (Resource.Layout.Main);
			//StartService(new Intent(this, typeof(HomeListenerService)));
			// Create your application here
		}
	}
}

