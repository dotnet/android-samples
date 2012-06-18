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
	[Activity]
	[IntentFilter(new []{Intent.ActionView}, Categories =new[]{ Intent.CategoryDefault }, DataMimeType = "vnd.android.cursor.item/vnd.iosched.vendor")]
	public class VendorDetailActivity : BaseSinglePaneActivity
	{
		protected override Fragment OnCreatePane ()
		{
			return new VendorDetailFragment();
		}
		
		protected override void OnPostCreate (Bundle savedInstanceState)
		{
			base.OnPostCreate (savedInstanceState);
			ActivityHelper.SetupSubActivity();
		}
	}
}

