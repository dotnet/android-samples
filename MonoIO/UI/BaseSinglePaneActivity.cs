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
using MonoIO.Utilities;
using Android.Support.V4.App;
using MonoIO.UI;
using Fragment = Android.Support.V4.App.Fragment;
using Android.Graphics;

namespace MonoIO
{
	[Activity (Label = "BaseSinglePaneActivity")]			
	public abstract class BaseSinglePaneActivity : BaseActivity
	{
		private Fragment fragment;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			
			
			SetContentView (Resource.Layout.activity_singlepane_empty);
			ActivityHelper.SetupActionBar (new Java.Lang.String (Title), new Color (0));
			
			var customTitle = Intent.GetStringExtra (Intent.ExtraTitle);
			var title = new Java.Lang.String (customTitle != null ? customTitle : Title);
			ActivityHelper.SetActionBarTitle (title);
			
			if (savedInstanceState == null) {
				fragment = OnCreatePane ();
				fragment.Arguments = IntentToFragmentArguments (Intent);
				
				SupportFragmentManager.BeginTransaction ().Add (Resource.Id.root_container, fragment).Commit ();
			}
		}
		
		protected abstract Fragment OnCreatePane ();
	}
}

