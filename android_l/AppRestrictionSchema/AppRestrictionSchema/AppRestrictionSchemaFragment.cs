
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Support.V4.App;
using Android.Support.Annotation;
using Android.OS;
using Android.Views;
using Android.Widget;

using CommonSampleLibrary;

using Fragment = Android.Support.V4.App.Fragment;

namespace AppRestrictionSchema
{
	/* Pressing the button on this fragment pops up a simple Toast message. The button is enabled or
	 * disabled according to the restrictions set by device/profile owner. You can use the 
	 * AppRestrictionEnforcer sample as a profile owner for this.
	 */ 
	public class AppRestrictionSchemaFragment : Fragment
	{
		// Tag for logger
		private string TAG = "AppRestrictionSchemaFragment";

		// UI components
		private TextView textSayHello;
		private Button buttonSayHello;

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			return inflater.Inflate (Resource.Layout.fragment_app_restriction_schema, container, false);
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			textSayHello = view.FindViewById<TextView> (Resource.Id.say_hello_explanation);
			buttonSayHello = view.FindViewById<Button> (Resource.Id.say_hello);
			buttonSayHello.Click += delegate {
				Toast.MakeText (Activity, Resource.String.message_hello, ToastLength.Short).Show ();
			};
		}

		public override void OnResume ()
		{
			base.OnResume ();

			// Update the UI according to the configured restrictions
			var restrictionsManager = (RestrictionsManager)Activity.GetSystemService (Context.RestrictionsService);
			Bundle restrictions = restrictionsManager.ApplicationRestrictions;
			UpdateUI (restrictions);
		}

		private void UpdateUI(Bundle restrictions)
		{
			if (CanSayHello (restrictions)) {
				textSayHello.SetText (Resource.String.explanation_can_say_hello_true);
				buttonSayHello.Enabled = true;
			} else {
				textSayHello.SetText (Resource.String.explanation_can_say_hello_false);
				buttonSayHello.Enabled = false;
			}
		}

		// Returns the current status of the restriction
		private bool CanSayHello(Bundle restrictions)
		{
			bool defaultValue = false;
			bool canSayHello = restrictions == null ? defaultValue :
				restrictions.GetBoolean("can_say_hello", defaultValue);
			Log.Debug (TAG, "canSayHello: " + canSayHello);
			return canSayHello;
		}
	}
}

