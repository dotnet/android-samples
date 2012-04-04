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

namespace ViewPagerIndicator
{
	[Activity (Label = "Titles/Default")]
	[IntentFilter (new[]{Intent.ActionMain}, Categories = new[]{ "mono.viewpagerindicator.sample" })]
	public class SampleTitlesDefault : BaseSampleActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			SetContentView (Resource.Layout.simple_titles);

			mAdapter = new TestTitleFragmentAdapter (SupportFragmentManager);
	
			mPager = FindViewById<ViewPager> (Resource.Id.pager);
			mPager.Adapter = mAdapter;
	
			mIndicator = FindViewById<TitlePageIndicator> (Resource.Id.indicator);
			mIndicator.SetViewPager (mPager);
		}
	}
}

