using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Views;

namespace MonoDroid.ApiDemo
{
	[Activity (Label = "Tutorials/Tab Layout", Theme="@android:style/Theme.NoTitleBar")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class TabLayoutTutorial : TabActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.TabLayoutTutorial);

			TabHost.TabSpec spec;     // Resusable TabSpec for each tab  
			Intent intent;            // Reusable Intent for each tab  

			// Create an Intent to launch an Activity for the tab (to be reused)  
			intent = new Intent (this, typeof (ArtistsActivity));
			intent.AddFlags (ActivityFlags.NewTask);

			// Initialize a TabSpec for each tab and add it to the TabHost  
			spec = TabHost.NewTabSpec ("artists");
			spec.SetIndicator ("Artists", Resources.GetDrawable (Resource.Drawable.ic_tab_artists));
			spec.SetContent (intent);
			TabHost.AddTab (spec);

			// Do the same for the other tabs  
			intent = new Intent (this, typeof (AlbumsActivity));
			intent.AddFlags (ActivityFlags.NewTask);

			spec = TabHost.NewTabSpec ("albums");
			spec.SetIndicator ("Albums", Resources.GetDrawable (Resource.Drawable.ic_tab_artists));
			spec.SetContent (intent);
			TabHost.AddTab (spec);

			intent = new Intent (this, typeof (SongsActivity));
			intent.AddFlags (ActivityFlags.NewTask);

			spec = TabHost.NewTabSpec ("songs");
			spec.SetIndicator ("Songs", Resources.GetDrawable (Resource.Drawable.ic_tab_artists));
			spec.SetContent (intent);
			TabHost.AddTab (spec);

			TabHost.CurrentTab = 2;
		}

		[Activity]
		public class ArtistsActivity : Activity
		{
			protected override void OnCreate (Bundle savedInstanceState)
			{
				base.OnCreate (savedInstanceState);

				TextView textview = new TextView (this);
				textview.Text = "This is the Artists tab";
				SetContentView (textview);
			}
		}

		[Activity]
		public class AlbumsActivity : Activity
		{
			protected override void OnCreate (Bundle savedInstanceState)
			{
				base.OnCreate (savedInstanceState);

				TextView textview = new TextView (this);
				textview.Text = "This is the Albums tab";
				SetContentView (textview);
			}
		}

		[Activity]
		public class SongsActivity : Activity
		{
			protected override void OnCreate (Bundle savedInstanceState)
			{
				base.OnCreate (savedInstanceState);

				TextView textview = new TextView (this);
				textview.Text = "This is the Songs tab";
				SetContentView (textview);
			}
		}
	}
}