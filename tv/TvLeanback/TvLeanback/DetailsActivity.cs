using System;
using Android.OS;
using Android.App;


namespace TvLeanback
{
	[Activity (Label = "DetailsActivity", Exported = true)]
	public class DetailsActivity : Activity
	{
		public static string SHARED_ELEMENT_NAME = "hero";
		public static string MOVIE = "Movie";
		public static string NOTIFICATION_ID = "NotificationId";

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.details);
		}
	}
}

