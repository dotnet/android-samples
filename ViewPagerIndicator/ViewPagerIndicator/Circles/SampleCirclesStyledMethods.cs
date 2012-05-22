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
	[Activity (Label = "Circles/Styled (via methods)", Theme = "@android:style/Theme.Light")]
	[IntentFilter (new[]{Intent.ActionMain}, Categories = new[]{ "mono.viewpagerindicator.sample" })]	
	public class SampleCirclesStyledMethods : BaseSampleActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			SetContentView (Resource.Layout.themed_circles);
		
			mAdapter = new TestFragmentAdapter (SupportFragmentManager);
			
			mPager = FindViewById<ViewPager> (Resource.Id.pager);
			mPager.Adapter = mAdapter;
			
			var indicator = FindViewById<CirclePageIndicator> (Resource.Id.indicator);
			mIndicator = indicator;
			indicator.SetViewPager (mPager);
			
			float density = Resources.DisplayMetrics.Density;
			indicator.SetBackgroundColor (Color.ParseColor ("#FFCCCCCC"));
			indicator.SetRadius (10 * density);
			indicator.SetPageColor (Color.ParseColor ("#880000FF"));
			indicator.SetFillColor (Color.ParseColor ("#FF888888"));
			indicator.SetStrokeColor (Color.ParseColor ("#FF000000"));
			indicator.SetStrokeWidth (2 * density);
		}
	}
}

