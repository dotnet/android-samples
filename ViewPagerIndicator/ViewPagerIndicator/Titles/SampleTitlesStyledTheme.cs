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
	[Activity (Label = "Titles/Styled (via theme)", Theme = "@style/StyledIndicators")]
	[IntentFilter (new[]{Intent.ActionMain}, Categories = new[]{ "mono.viewpagerindicator.sample" })]
	public class SampleTitlesStyledTheme : BaseSampleActivity
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
		}
	}
}