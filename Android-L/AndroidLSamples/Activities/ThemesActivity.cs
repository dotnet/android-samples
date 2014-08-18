using Android.App;
using Android.OS;
namespace AndroidLSamples
{
	[Activity (Label = "Themed Controls", ParentActivity=typeof(HomeActivity))]			
	public class ThemesActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.activity_themes);
			ActionBar.SetDisplayHomeAsUpEnabled (true);
			ActionBar.SetDisplayShowHomeEnabled (true);
			ActionBar.SetIcon (Android.Resource.Color.Transparent);
		}

		public override bool OnCreateOptionsMenu (Android.Views.IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.menu2, menu);
			return base.OnCreateOptionsMenu (menu);
		}
	}
}

