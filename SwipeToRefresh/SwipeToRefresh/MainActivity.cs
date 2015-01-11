using System;
using System.Linq;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Android.Support.V4.Widget;
using Android.Util;

namespace SwipeToRefresh
{
	[Activity (Label = "Swipe Refresh", MainLauncher = true, Theme = "@style/XamActionBarTheme")]
	public class MainActivity : Android.Support.V4.App.FragmentActivity
	{
		PostListFragment forum;
		SwipeRefreshLayout refresher;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Main);

			forum = PostListFragment.Instantiate ("android", "Android");
			SupportFragmentManager.BeginTransaction ()
				.Add (Resource.Id.container, forum, "post-list")
				.Commit ();

			refresher = FindViewById<SwipeRefreshLayout> (Resource.Id.refresher);
			refresher.SetColorScheme (Resource.Color.xam_dark_blue,
			                          Resource.Color.xam_purple,
			                          Resource.Color.xam_gray,
			                          Resource.Color.xam_green);
			refresher.Refresh += HandleRefresh;
		}

		async void HandleRefresh (object sender, EventArgs e)
		{
			await forum.FetchItems (clear: true);
			refresher.Refreshing = false;
		}
	}
}


