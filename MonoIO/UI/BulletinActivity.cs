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
using Android.Support.V4.App;
using MonoIO.UI;
using Fragment = Android.Support.V4.App.Fragment;

namespace MonoIO
{
	[Activity (Label = "@string/title_bulletin")]			
	public class BulletinActivity : BaseSinglePaneActivity
	{
		protected override Fragment OnCreatePane ()
		{
			return new BulletinFragment();
		}
		
		protected override void OnPostCreate (Bundle savedInstanceState)
		{
			base.OnPostCreate (savedInstanceState);
			ActivityHelper.SetupSubActivity();
		}
	}
}

