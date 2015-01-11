using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

/**
 * A sample activity demonstrating the "done button" alternative action bar presentation. For a more
 * detailed description see {@link R.string.done_button_description}.
 */

namespace DoneBar
{
	[Activity (Label = "DoneButtonActivity", Theme="@style/Theme.Sample")]			
	public class DoneButtonActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Inflate a "Done" custom action bar view to serve as the "Up" affordance.
			var inflater = (LayoutInflater) ActionBar.ThemedContext.GetSystemService (Context.LayoutInflaterService);
			View customActionBarView = inflater.Inflate (Resource.Layout.actionbar_custom_view_done, null);

			customActionBarView.FindViewById (Resource.Id.actionbar_done).Click += delegate {
				// "Done"
				Finish();
			};

			// Show the custom action bar view and hide the normal Home icon and title.

			ActionBar.SetDisplayOptions (ActionBarDisplayOptions.ShowCustom, ActionBarDisplayOptions.ShowCustom
				| ActionBarDisplayOptions.ShowHome | ActionBarDisplayOptions.ShowTitle);

			ActionBar.CustomView = customActionBarView;

			SetContentView (Resource.Layout.activity_done_button);
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			base.OnCreateOptionsMenu (menu);
			MenuInflater.Inflate (Resource.Menu.cancel, menu);
			return true;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case Resource.Id.cancel:
				// "Cancel"
				Finish ();
				return true;
			}
			return base.OnOptionsItemSelected (item);
		}
	}
}

