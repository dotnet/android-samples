using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content.Res;
using Android.Support.Wearable.Views;

namespace GridViewPagerSample
{
	[Activity (Label = "GridViewPager", MainLauncher = true, Icon = "@drawable/ic_launcher")]
	public class MainActivity : Activity, View.IOnApplyWindowInsetsListener
	{
		Resources res;
		GridViewPager pager;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.activity_main);
			res = Resources;
			pager = (GridViewPager)FindViewById (Resource.Id.pager);
			pager.SetOnApplyWindowInsetsListener (this);
			pager.Adapter = new SimpleGridPagerAdapter (this, FragmentManager);
		}

		public WindowInsets OnApplyWindowInsets (View v, WindowInsets insets)
		{
			// Adjust page margins
			// A little extra horizontal spacing between pages looks a but less crouded on a round display
			bool round = insets.IsRound;
			int rowMargin = res.GetDimensionPixelOffset (Resource.Dimension.page_row_margin);
			int colMargin = res.GetDimensionPixelOffset (
				                round ? Resource.Dimension.page_column_margin_round : Resource.Dimension.page_column_margin);
			pager.SetPageMargins (rowMargin, colMargin);
			return insets;

		}
	}
}


