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
	[Activity (Label = "Circles/Snap")]
	[IntentFilter (new[]{Intent.ActionMain}, Categories = new[]{ "mono.viewpagerindicator.sample" })]
	public class SampleCirclesSnap : BaseSampleActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			SetContentView (Resource.Layout.simple_circles);
			
			mAdapter = new TestFragmentAdapter (SupportFragmentManager);
			
			mPager = FindViewById<ViewPager> (Resource.Id.pager);
			mPager.Adapter = mAdapter;
			var indicator = FindViewById<CirclePageIndicator> (Resource.Id.indicator);
			mIndicator = indicator;
			indicator.SetViewPager (mPager);
			indicator.SetSnap (true);
		}
	}
}

