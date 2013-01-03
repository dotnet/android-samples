
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
using ActionbarSherlock.App;
using ActionbarSherlock.View;

using IMenu = global::ActionbarSherlock.View.IMenu;
using IMenuItem = global::ActionbarSherlock.View.IMenuItem;
using MenuItem = global::ActionbarSherlock.View.MenuItem;
using ISubMenu = global::ActionbarSherlock.View.ISubMenu;

namespace Mono.ActionbarsherlockTest
{
	[Activity (Name = "mono.actionbarsherlocktest.Simple", Label = "@string/simple")]
	[IntentFilter (new string [] { Intent.ActionMain },
		Categories = new string [] { Constants.DemoCategory })]
	public class Simple : SherlockActivity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			this.SetTheme (SampleList.THEME); //Used for theme switching in samples
			SetContentView (Resource.Layout.text);
			FindViewById<TextView>(Resource.Id.text).SetText (Resource.String.simple_content);
		}
	}
}

