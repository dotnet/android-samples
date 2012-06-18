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
using ListFragment = Android.Support.V4.App.ListFragment;
using Fragment = Android.Support.V4.App.Fragment;

namespace MonoIO
{
	[Activity (Label = "@string/title_tracks")]
	[IntentFilter(new []{Intent.ActionView}, Categories =new[]{ Intent.CategoryDefault }, DataMimeType = "vnd.android.cursor.dir/vnd.iosched.track")]
	public class TracksActivity : BaseSinglePaneActivity
	{
		protected override Fragment OnCreatePane ()
		{
			return new TracksFragment();
		}
		
		protected override void OnPostCreate (Bundle savedInstanceState)
		{
			base.OnPostCreate (savedInstanceState);
			ActivityHelper.SetupSubActivity();
		}
	}
}

