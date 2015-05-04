

using Android.App;
using Android.OS;
using Android.Views;
using Android.Support.V7.App;
using Android.Support.V7.Widget;

namespace HelloToolbar
{
	[Activity (Label = "DetailActivity")]			
	public class DetailActivity : AppCompatActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.detail);
			var toolbar = FindViewById<Toolbar> (Resource.Id.toolbar);

			//Toolbar will now take on default actionbar characteristics
			SetSupportActionBar (toolbar);

			SupportActionBar.Title = "Hello from Toolbar";

			SupportActionBar.SetDisplayHomeAsUpEnabled (true);
			SupportActionBar.SetHomeButtonEnabled (true);
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			if (item.ItemId == Android.Resource.Id.Home)
				Finish ();

			return base.OnOptionsItemSelected (item);
		}
	
	}
}

