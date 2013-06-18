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
	[Activity (Label = "DrawerLayoutActivity")]			
	public class DrawerLayoutActivity : Activity
	{
		private string[] items; 
		private DrawerLayout mDrawerLayout;
		private ListView mDrawerList;
		private TextView mTextView;
		private ActionBarDrawerToggle mDrawerToggle;
		private ActionBarHelper mActionBar;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.DrawerLayout);


			mActionBar = createActionBarHelper ();
			mActionBar.init ();

			items = new string[] { "Vegetables","Fruits","Flower Buds","Legumes","Bulbs","Tubers" };

			mDrawerList =FindViewById<ListView> (Resource.Id.left_drawer);
			mTextView =FindViewById <TextView> (Resource.Id.content_text_DrawerLayout);

			mDrawerList.Adapter = new ArrayAdapter<string>(this,Android.Resource.Layout.SimpleListItem1, items);
			mDrawerList.ItemClick += OnDrawerLIstItemClick;

			mDrawerLayout = (DrawerLayout)FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
			mDrawerLayout.DrawerOpened += HandleDrawerOpened;
			mDrawerLayout.DrawerClosed += HandleDrawerClosed;

			// ActionBarDrawerToggle provides convenient helpers for tying together the
			// prescribed interactions between a top-level sliding drawer and the action bar.
			mDrawerToggle = new ActionBarDrawerToggle (this, 
			                                           mDrawerLayout,
			                                           Resource.Drawable.ic_drawer, 
			                                           Resource.String.drawer_open, 
			                                           Resource.String.drawer_close);

		}

		void HandleDrawerClosed (object sender, DrawerLayout.DrawerClosedEventArgs e)
		{
			mActionBar.onDrawerClosed ();
		}

		void HandleDrawerOpened (object sender, DrawerLayout.DrawerOpenedEventArgs e)
		{
			mActionBar.onDrawerOpened ();
		}

		protected override void OnPostCreate (Bundle savedInstanceState)
		{
			base.OnPostCreate (savedInstanceState);

			// Sync the toggle state after onRestoreInstanceState has occurred.
			mDrawerToggle.SyncState ();
		}

		public override void OnConfigurationChanged (Android.Content.Res.Configuration newConfig)
		{
			base.OnConfigurationChanged (newConfig);
			mDrawerToggle.OnConfigurationChanged (newConfig);
		}
	
		public override bool OnOptionsItemSelected (IMenuItem item)
		{

	        //The action bar home/up action should open or close the drawer.
	        //mDrawerToggle will take care of this.
			if (mDrawerToggle.OnOptionsItemSelected (item)) {
				return true;
			}

			return base.OnOptionsItemSelected (item);
		}

		void OnDrawerLIstItemClick(object sender, AdapterView.ItemClickEventArgs e)
		{
			var item =(string)(sender as ListView).GetItemAtPosition (e.Position);
			mTextView.Text = item;
			mDrawerLayout.CloseDrawer (mDrawerList);
		}

		//Create a compatible helper that will manipulate the action bar if available.
		private ActionBarHelper createActionBarHelper() {
			if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.IceCreamSandwich) {
				return new ActionBarHelperICS(this.ActionBar);
			} else {
				return new ActionBarHelper();
			}
		}

		//Stub action bar helper; this does nothing.
		private class ActionBarHelper {
			public virtual void init() {}
			public virtual void onDrawerClosed() {}
			public virtual void onDrawerOpened() {}
			public virtual void setTitle(string mTitle){}
		}


		//Action bar helper for use on ICS and newer devices.
		private class ActionBarHelperICS : ActionBarHelper {
			private  ActionBar mActionBar;
			private string mDrawerTitle = "Drawer Open";
			private string mTitle = "Drawer Close";

			public ActionBarHelperICS(ActionBar mActionBar) 
			{
				this.mActionBar = mActionBar;
			}

			public override void init() 
			{
				mActionBar.SetDisplayHomeAsUpEnabled(true);
				mActionBar.SetHomeButtonEnabled(true);
			}
			public override void onDrawerClosed() 
			{
				mActionBar.Title = mTitle;
			}

			public override void onDrawerOpened() 
			{
				mActionBar.Title = mDrawerTitle;
			}
			public override void setTitle(string mTitle)
			{
				mActionBar.Title = mTitle;
			}
		}

	}
}

