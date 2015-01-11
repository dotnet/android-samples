using Android.App;
using Android.OS;

namespace AndroidLSamples
{
	[Activity (Label = "CardView Sample", ParentActivity=typeof(HomeActivity))]			
	public class CardActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.activity_card);
			ActionBar.SetDisplayHomeAsUpEnabled (true);
			ActionBar.SetDisplayShowHomeEnabled (true);
			ActionBar.SetIcon (Android.Resource.Color.Transparent);
		}
	}
}

