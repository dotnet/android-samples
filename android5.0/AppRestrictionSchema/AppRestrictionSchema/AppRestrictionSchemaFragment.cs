
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

using CommonSampleLibrary;


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
		static readonly string KEY_CAN_SAY_HELLO = "can_say_hello";

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
			ResolveRestrictions ();
		}

		void ResolveRestrictions ()
		{
			var manager = (RestrictionsManager) Activity.GetSystemService (Context.RestrictionsService);
			Bundle restrictions = manager.ApplicationRestrictions;
			IList<RestrictionEntry> entries = manager.GetManifestRestrictions (Activity.ApplicationContext.PackageName);

			foreach (RestrictionEntry entry in entries) {
				String key = entry.Key;
				Log.Debug (TAG, "key: " + key);
				if (key == KEY_CAN_SAY_HELLO) {
					UpdateCanSayHello (entry, restrictions);
				}
			}
		}

		void UpdateCanSayHello (RestrictionEntry entry, Bundle restrictions)
		{
			bool canSayHello;
			if (restrictions == null || !restrictions.ContainsKey (KEY_CAN_SAY_HELLO))
				canSayHello = entry.SelectedState;
			else
				canSayHello = restrictions.GetBoolean (KEY_CAN_SAY_HELLO);

			textSayHello.SetText (canSayHello ? Resource.String.explanation_can_say_hello_true :
				Resource.String.explanation_can_say_hello_false);
			buttonSayHello.Enabled = canSayHello;
		}
	}
}

