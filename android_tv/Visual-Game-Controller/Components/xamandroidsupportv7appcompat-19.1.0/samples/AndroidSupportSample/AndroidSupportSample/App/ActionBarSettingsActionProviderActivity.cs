using System;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V4.View;
using Android.Content;
using Android.Widget;

namespace AndroidSupportSample
{
	[Android.App.Activity (Label = "@string/action_bar_settings_action_provider", Theme = "@style/Theme.AppCompat")]			
	public class ActionBarSettingsActionProviderActivity : ActionBarActivity
	{
		public override bool OnCreateOptionsMenu (Android.Views.IMenu menu)
		{
			base.OnCreateOptionsMenu (menu);
			MenuInflater.Inflate (Resource.Menu.action_bar_settings_action_provider, menu);
			return true;
		}

		public override bool OnOptionsItemSelected (Android.Views.IMenuItem item)
		{
			// If this callback does not handle the item click, onPerformDefaultAction
			// of the ActionProvider is invoked. Hence, the provider encapsulates the
			// complete functionality of the menu item.
			Toast.MakeText (this, Resource.String.action_bar_settings_action_provider_no_handling,
				ToastLength.Short).Show ();
			return false;
		}

		public class SettingsActionProvider : ActionProvider
		{
			// An intent for launching the system settings.
			private static Intent sSettingsIntent = new Intent (Android.Provider.Settings.ActionSettings);

			public SettingsActionProvider (Context context) : base (context)
			{
			
			}

			public override Android.Views.View OnCreateActionView ()
			{
				// Inflate the action view to be shown on the action bar.
				var layoutInflater = Android.Views.LayoutInflater.FromContext (Context);
				var view = layoutInflater.Inflate (Resource.Layout.action_bar_settings_action_provider, null);
				ImageButton button = (ImageButton)view.FindViewById (Resource.Id.button);
				// Attach a click listener for launching the system settings.
				button.Click += (sender, e) => {
					Context.StartActivity (sSettingsIntent);
				};

				return view;
			}

			public override bool OnPerformDefaultAction ()
			{
				// This is called if the host menu item placed in the overflow menu of the
				// action bar is clicked and the host activity did not handle the click.
				Context.StartActivity (sSettingsIntent);
				return true;
			}
		}
	}
}

