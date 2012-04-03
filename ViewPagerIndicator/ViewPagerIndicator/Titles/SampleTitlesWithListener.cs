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
using Android.Support.V4.View;
using Android.Graphics;

namespace ViewPagerIndicator
{
	[Activity (Label = "Titles/With Listener")]
	[IntentFilter (new[]{Intent.ActionMain}, Categories = new[]{ "mono.viewpagerindicator.sample" })]
	public class SampleTitlesWithListener : BaseSampleActivity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			
			SetContentView (Resource.Layout.simple_titles);
			
			mAdapter = new TestTitleFragmentAdapter (SupportFragmentManager);
			
			mPager = FindViewById<ViewPager> (Resource.Id.pager);
			mPager.Adapter = mAdapter;
			
			mIndicator = FindViewById<TitlePageIndicator> (Resource.Id.indicator);
			mIndicator.SetViewPager (mPager);
			
			mIndicator.SetOnPageChangeListener (new MyPageChangeListener (this));
		}
		
		public class MyPageChangeListener : Java.Lang.Object, ViewPager.IOnPageChangeListener
		{
			Context _context;
			
			public MyPageChangeListener (Context context)
			{
				_context = context;	
			}
			
			#region IOnPageChangeListener implementation
			public void OnPageScrollStateChanged (int p0)
			{
			}

			public void OnPageScrolled (int p0, float p1, int p2)
			{
			}

			public void OnPageSelected (int position)
			{
				Toast.MakeText (_context, "Changed to page " + position, ToastLength.Short).Show ();
			}
			#endregion
		}
	}
}