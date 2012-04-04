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
using Android.Support.V4.View;

namespace ViewPagerIndicator
{
	[Activity (Label = "BaseSampleActivity")]			
	public class BaseSampleActivity : FragmentActivity
	{
		private Random RANDOM = new Random ();
		public TestFragmentAdapter mAdapter;
		public ViewPager mPager;
		public PageIndicator mIndicator;
		
		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.menu, menu);
			return true;
		}
		
		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case Resource.Id.random:
				int page = RANDOM.Next (mAdapter.Count);
				Toast.MakeText (this, "Changing to page " + page, ToastLength.Short);
				mPager.CurrentItem = page;
				return true;

			case Resource.Id.add_page:
				Console.WriteLine ("Adapter count " + mAdapter.Count);
				if (mAdapter.Count < 10) {
					mAdapter.SetCount (mAdapter.Count + 1);
					mIndicator.NotifyDataSetChanged ();
				}
				return true;

			case Resource.Id.remove_page:
				
				Console.WriteLine ("Remove page " + mAdapter.Count);
				if (mAdapter.Count > 1) {
					mAdapter.SetCount (mAdapter.Count - 1);
					mIndicator.NotifyDataSetChanged ();
				}
				return true;
			}
			return base.OnOptionsItemSelected (item);
		}
	}
}

