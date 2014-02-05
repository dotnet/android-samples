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
	[Activity (Label = "SlidingPaneLayoutActivity")]			
	public class SlidingPaneLayoutActivity : Activity
	{
		private SlidingPaneLayout mSlidingLayout;
		private ListView mList;
		private TextView mContent;
		private ActionBarHelper mActionBar;

		private string[] items;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here

			SetContentView (Resource.Layout.SlidingPaneLayout);

			mSlidingLayout = (SlidingPaneLayout)FindViewById<SlidingPaneLayout> (Resource.Id.sliding_pane_layout);
			mContent = (TextView)FindViewById<TextView> (Resource.Id.content_text_slidingPaneLayout);

			mActionBar = createActionBarHelper ();
			mActionBar.init ();

			mSlidingLayout.ViewTreeObserver.GlobalLayout += HandleGlobalLayout;

			mSlidingLayout.PanelOpened += (sender, e) => {
				mActionBar.onPanelOpened();

			};
			mSlidingLayout.PanelClosed += (sender, e) => {
				mActionBar.onPanelClosed();
			};

			mList = (ListView)FindViewById<ListView> (Resource.Id.left_pane);
			items = new string[] { "Vegetables","Fruits","Flower Buds","Legumes","Bulbs","Tubers" };
			mList.Adapter = new ArrayAdapter<string>(this,Android.Resource.Layout.SimpleListItem1, items);
			mList.ItemClick += HandleItemClick;
		}

		void HandleItemClick (object sender, AdapterView.ItemClickEventArgs e)
		{
			string selectItem = (string)(sender as ListView).GetItemAtPosition (e.Position);
			mContent.Text = selectItem;
			mActionBar.setTitle (selectItem);
			mSlidingLayout.SmoothSlideClosed ();
		}

		void HandleGlobalLayout (object sender, EventArgs e)
		{
			//This global layout handler is used to fire an event after first layout occurs
			//and then it is removed. This gives us a chance to configure parts of the UI
			//that adapt based on available space after they have had the opportunity to measure
			//and layout.
			mActionBar.onFirstLayout ();
			mSlidingLayout.ViewTreeObserver.GlobalLayout -= HandleGlobalLayout;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{

			if (item.ItemId == Android.Resource.Id.Home && !mSlidingLayout.IsOpen) 
			{
				mSlidingLayout.SmoothSlideOpen ();
				return true;
			}

			return base.OnOptionsItemSelected (item);
		}

		//Create a compatible helper that will manipulate the action bar if available.
		private ActionBarHelper createActionBarHelper() {
			if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.IceCreamSandwich) {
				return new ActionBarHelperICS(this.ActionBar,mSlidingLayout);
			} else {
				return new ActionBarHelper();
			}
		}

		//Stub action bar helper; this does nothing.
		private class ActionBarHelper {
			public virtual void init() {}
			public virtual void onPanelClosed() {}
			public virtual void onPanelOpened() {}
			public virtual void onFirstLayout() {}
			public virtual void setTitle(string title) {}
		}

		//Action bar helper for use on ICS and newer devices.
		private class ActionBarHelperICS : ActionBarHelper {
			private  ActionBar mActionBar;
			private string mDrawerTitle = "Pane Open";
			private string mTitle = "Pane Close";
			private SlidingPaneLayout mSlidingLayout;

			public ActionBarHelperICS(ActionBar mActionBar, SlidingPaneLayout mSlidingLayout) {
				this.mActionBar = mActionBar;
				this.mSlidingLayout = mSlidingLayout;
			}

			public override void init() {
				mActionBar.SetDisplayHomeAsUpEnabled(true);
				mActionBar.SetHomeButtonEnabled(true);
			}
			public override void onPanelClosed() {
				base.onPanelClosed();
				mActionBar.SetDisplayHomeAsUpEnabled(true);
				mActionBar.SetHomeButtonEnabled(true);
				mActionBar.Title=mTitle;
			}

			public override void onPanelOpened() {
				base.onPanelOpened();
				mActionBar.SetHomeButtonEnabled(false);
				mActionBar.SetDisplayHomeAsUpEnabled(false);
				mActionBar.Title=mDrawerTitle;
			}

			public override void onFirstLayout() {
				//Do something you need	
			}
			public override void setTitle(string title) {
				mTitle = title;
			}
		}
	}


}

