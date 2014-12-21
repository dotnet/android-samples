using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace HelloToolbar
{
	[Activity (Label = "HelloToolbar", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			var toolbar = FindViewById<Toolbar> (Resource.Id.toolbar);

			//Toolbar will now take on default actionbar characteristics
			SetActionBar (toolbar);

			ActionBar.Title = "Hello from Toolbar";


			var toolbarBottom = FindViewById<Toolbar> (Resource.Id.toolbar_bottom);

			toolbarBottom.Title = "Photo Editing";
			toolbarBottom.InflateMenu (Resource.Menu.photo_edit);
			toolbarBottom.MenuItemClick += (sender, e) => {
				Toast.MakeText(this, "Bottom toolbar pressed: " + e.Item.TitleFormatted, ToastLength.Short).Show();
			};
		}

		/// <Docs>The options menu in which you place your items.</Docs>
		/// <returns>To be added.</returns>
		/// <summary>
		/// This is the menu for the Toolbar/Action Bar to use
		/// </summary>
		/// <param name="menu">Menu.</param>
		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.home, menu);
			return base.OnCreateOptionsMenu (menu);
		}
		public override bool OnOptionsItemSelected (IMenuItem item)
		{	
			Toast.MakeText(this, "Top ActionBar pressed: " + item.TitleFormatted, ToastLength.Short).Show();
			return base.OnOptionsItemSelected (item);
		}
	}
}


