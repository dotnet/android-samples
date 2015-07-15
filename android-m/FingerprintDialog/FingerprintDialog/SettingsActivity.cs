using System;
using Android.App;
using Android.Preferences;

namespace FingerprintDialog
{
	[Activity (Label = "@string/action_settings")]
	public class SettingsActivity : Activity
	{
		protected override void OnCreate (Android.OS.Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			FragmentManager.BeginTransaction ().Replace (Android.Resource.Id.Content, new SettingsFragment ()).Commit ();
		}

		public class SettingsFragment : PreferenceFragment
		{
			public override void OnCreate (Android.OS.Bundle savedInstanceState)
			{
				base.OnCreate (savedInstanceState);
				AddPreferencesFromResource (Resource.Xml.preferences);
			}
		}
	}
}

