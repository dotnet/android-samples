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
using Android.Support.V4;
using Android.Support.V4.App;
using Android.Support.V4.Widget;
namespace SupportV4R13
{
	public class MyActionBarDrawerToggle : ActionBarDrawerToggle
	{
		Activity activity;
		public MyActionBarDrawerToggle (Activity activity, DrawerLayout drawerLayout, int drawerImageRes, int openDrawerContentDescRes, int closeDrawerContentDescRes) 
			: base(activity, drawerLayout, drawerImageRes, openDrawerContentDescRes, closeDrawerContentDescRes)
		{
			this.activity = activity;
		}
		public override void OnDrawerClosed (View p0)
		{
			activity.ActionBar.Title = "Drawer Closed";
			activity.InvalidateOptionsMenu ();
		}
		public override void OnDrawerOpened (View p0)
		{
			activity.ActionBar.Title = "Drawer Opened";
			activity.InvalidateOptionsMenu ();
		}
	}
}

