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
	[Activity (Label = "Titles/Center Click Listener")]
	[IntentFilter (new[]{Intent.ActionMain}, Categories = new[]{ "mono.viewpagerindicator.sample" })]
	public class SampleTitlesCenterClickListener : BaseSampleActivity, TitlePageIndicator.OnCenterItemClickListener
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			
			SetContentView (Resource.Layout.simple_titles);
			
			mAdapter = new TestTitleFragmentAdapter (SupportFragmentManager);
			
			mPager = FindViewById<ViewPager> (Resource.Id.pager);
			mPager.Adapter = mAdapter;
			
			var indicator = FindViewById<TitlePageIndicator> (Resource.Id.indicator);
			indicator.SetViewPager (mPager);
			indicator.SetFooterIndicatorStyle (IndicatorStyle.Underline);
			indicator.SetOnCenterItemClickListener (this);
			mIndicator = indicator;
		}
		
		public void OnCenterItemClick (int position)
		{
			Toast.MakeText (this, "You clicked the center title!", ToastLength.Short).Show ();
		}	
	}
}