using System;
using Android.OS;
using Android.App;


namespace TvLeanback
{
	[Activity (Label = "DetailsActivity", Exported = true)]
	public class DetailsActivity : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.details);
		}
	}
}

