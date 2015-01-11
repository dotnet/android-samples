using System;
using Android.App;
using Android.OS;

namespace TvLeanback
{
	[Activity (Label = "SearchActivity", Exported = true)]
	public class SearchActivity: Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.search);
		}
	}
}

