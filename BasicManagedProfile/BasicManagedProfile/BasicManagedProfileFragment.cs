using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.App.Admin;

namespace BasicManagedProfile
{
	public class BasicManagedProfileFragment : Fragment, View.IOnClickListener, CompoundButton.IOnCheckedChangeListener
	{
		int FlagToManagedProfile = Android.App.Admin.DevicePolicyManager.FlagToManagedProfile;
		int FlagToPrimaryUser = Android.App.Admin.DevicePolicyManager.FlagToPrimaryUser;

		/** Package names of calculator */
		private string[] PackageNamesCalculator = {
			"com.android.calculator2"
		};

		/** Package names of Chrome */
		private string[] PackageNamesChrome = {
			"com.android.chrome",
			"com.google.android.apps.chrome_dev",
			"com.chrome.canary",
			"com.chrome.beta",
		};

		private Button mButtonRemoveProfile;

		/** Whether the calculator app is enabled in this profile */
		private bool mCalculatorEnabled;

		/** Whether Chrome is enabled in this profile */
		private bool mChromeEnabled;

		public BasicManagedProfileFragment ()
		{
		}

		public static BasicManagedProfileFragment NewInstance ()
		{
			return new BasicManagedProfileFragment ();
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container,
		                                   Bundle savedInstanceState)
		{
			return inflater.Inflate (Resource.Layout.fragment_main, container, false);
		}

		public override void OnAttach (Activity activity)
		{
			base.OnAttach (activity);
			// Gets an instance of DevicePolicyManager
			DevicePolicyManager manager =
				(DevicePolicyManager)activity.GetSystemService (Context.DevicePolicyService);
			// Retrieves whether the calculator app is enabled in this profile
			mCalculatorEnabled = !manager.IsApplicationBlocked (
				BasicDeviceAdminReceiver.GetComponentName (activity), PackageNamesCalculator [0]);
			// Retrieves whether Chrome is enabled in this profile
			mChromeEnabled = false;
			foreach (String packageName in PackageNamesChrome) {
				if (!manager.IsApplicationBlocked (
					    BasicDeviceAdminReceiver.GetComponentName (activity), packageName)) {
					mChromeEnabled = true;
					return;
				}
			}
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			// Bind event listeners and initial states
			view.FindViewById (Resource.Id.set_chrome_restrictions).SetOnClickListener (this);
			view.FindViewById (Resource.Id.enable_forwarding).SetOnClickListener (this);
			view.FindViewById (Resource.Id.disable_forwarding).SetOnClickListener (this);
			view.FindViewById (Resource.Id.send_intent).SetOnClickListener (this);
			mButtonRemoveProfile = view.FindViewById<Button> (Resource.Id.remove_profile);
			mButtonRemoveProfile.SetOnClickListener (this);
			var toggleCalculator = (Switch)view.FindViewById (Resource.Id.toggle_calculator);
			toggleCalculator.Checked = mCalculatorEnabled;
			toggleCalculator.SetOnCheckedChangeListener (this);
			var toggleChrome = view.FindViewById<Switch> (Resource.Id.toggle_chrome);
			toggleChrome.Checked = mChromeEnabled;
			toggleChrome.SetOnCheckedChangeListener (this);
		}

		public void OnClick (View view)
		{
			switch (view.Id) {
			case Resource.Id.set_chrome_restrictions:
				SetChromeRestrictions ();
				break;
			case Resource.Id.enable_forwarding:
				enableForwarding ();
				break;
			case Resource.Id.disable_forwarding:
				disableForwarding ();
				break;
			case Resource.Id.send_intent:
				sendIntent ();
				break;
			case Resource.Id.remove_profile:
				mButtonRemoveProfile.Enabled = false;
				RemoveProfile ();
				break;
			}
		}

		public void OnCheckedChanged (CompoundButton compoundButton, bool check)
		{
			switch (compoundButton.Id) {
			case Resource.Id.toggle_calculator:
				{
					SetAppEnabled (PackageNamesCalculator, check);
					mCalculatorEnabled = check;
					break;
				}
			case Resource.Id.toggle_chrome:
				{
					SetAppEnabled (PackageNamesChrome, check);
					mChromeEnabled = check;
					break;
				}
			}
		}

		/**
	     * Enables or disables the specified app in this profile.
	     *
	     * @param packageNames The package names of the target app.
	     * @param enabled Pass true to enable the app.
	     */
		private void SetAppEnabled (String[] packageNames, bool enabled)
		{
			Activity activity = this.Activity;
			if (null == activity) {
				return;
			}
			DevicePolicyManager manager =
				(DevicePolicyManager)activity.GetSystemService (Context.DevicePolicyService);
			foreach (String packageName in packageNames) {
				// This is how you can enable or disable an app in a managed profile.
				manager.SetApplicationBlocked (BasicDeviceAdminReceiver.GetComponentName (activity),
					packageName, !enabled);
			}
			Toast.MakeText (activity, enabled ? "Enabled" : "Disabled", ToastLength.Short).Show ();
		}

		/**
	     * Sets restrictions to Chrome
	     */
		private void SetChromeRestrictions ()
		{
			Activity activity = Activity; //the activity to which this fragment belongs
			if (null == this.Activity) {
				return;
			}
			var manager =
				(DevicePolicyManager)activity.GetSystemService (Context.DevicePolicyService);
			var settings = new Bundle ();
			settings.PutString ("EditBookmarksEnabled", "false");
			settings.PutString ("IncognitoModeAvailability", "1");
			settings.PutString ("ManagedBookmarks",
				"[{\"name\": \"Chromium\", \"url\": \"http://chromium.org\"}, " +
				"{\"name\": \"Google\", \"url\": \"https://www.google.com\"}]");
			settings.PutString ("DefaultSearchProviderEnabled", "true");
			settings.PutString ("DefaultSearchProviderName", "\"LMGTFY\"");
			settings.PutString ("DefaultSearchProviderSearchURL",
				"\"http://lmgtfy.com/?q={searchTerms}\"");
			settings.PutString ("URLBlacklist", "[\"example.com\", \"example.org\"]");
			StringBuilder message = new StringBuilder ("Setting Chrome restrictions:");
			foreach (string key in settings.KeySet()) {
				message.Append ("\n");
				message.Append (key);
				message.Append (": ");
				message.Append (settings.GetString (key));
			}
			ScrollView view = new ScrollView (activity);
			TextView text = new TextView (activity);
			text.Text = message.ToString ();
			int size = (int)activity.Resources.GetDimension (Resource.Dimension.activity_horizontal_margin);
			view.SetPadding (size, size, size, size);
			view.AddView (text);
			new AlertDialog.Builder (activity)
				.SetView (view)
				.SetNegativeButton (Android.Resource.String.Cancel, (s, e) => {
			})//using null causes ambiguity
				.SetPositiveButton (Android.Resource.String.Ok, ((object sender, DialogClickEventArgs e) => {
				manager.SetApplicationRestrictions
					(BasicDeviceAdminReceiver.GetComponentName (activity),
					"", settings);
				Toast.MakeText (this.Activity, "Restrictions set.",
					ToastLength.Short).Show ();
			})).Show ();
		}

		/*
	     * Enables forwarding of share intent between private account and managed profile.
	     */
		private void enableForwarding ()
		{
			Activity activity = Activity; //the activity to which this fragment belongs
			if (null == activity || activity.IsFinishing) {
				return;
			}
			DevicePolicyManager manager =
				(DevicePolicyManager)activity.GetSystemService (Context.DevicePolicyService);
			try {
				IntentFilter filter = new IntentFilter (Intent.ActionSend);
				filter.AddDataType ("text/plain");
				filter.AddDataType ("image/jpeg");
				// This is how you can register an IntentFilter as allowed pattern of Intent forwarding
				manager.AddForwardingIntentFilter (BasicDeviceAdminReceiver.GetComponentName (activity),
					filter,
					FlagToPrimaryUser | FlagToManagedProfile);
			} catch (IntentFilter.MalformedMimeTypeException e) {
				e.PrintStackTrace ();
			}
		}

		/**
	     * Disables forwarding of all intents.
	     */
		private void disableForwarding ()
		{
			Activity activity = Activity;
			if (null == activity || activity.IsFinishing) {
				return;
			}
			DevicePolicyManager manager =
				(DevicePolicyManager)activity.GetSystemService (Context.DevicePolicyService);
			manager.ClearForwardingIntentFilters (BasicDeviceAdminReceiver.GetComponentName (activity));
		}

		/**
	     * Sends a sample intent.
	     */
		private void sendIntent ()
		{
			Activity activity = this.Activity;
			if (null == activity || activity.IsFinishing) {
				return;
			}
			DevicePolicyManager manager =
				(DevicePolicyManager)activity.GetSystemService (Context.DevicePolicyService);
			Intent intent = new Intent (Intent.ActionSend);
			intent.SetType ("text/plain");
			intent.PutExtra (Intent.ExtraText,
				manager.IsProfileOwnerApp (activity.ApplicationContext.PackageName)
				? "From the managed account" : "From the primary account");
			StartActivity (intent);
		}

		/**
	     * Wipes out all the data related to this managed profile.
	     */
		private void RemoveProfile ()
		{
			Activity activity = Activity;
			if (null == activity || activity.IsFinishing) {
				return;
			}
			DevicePolicyManager manager =
				(DevicePolicyManager)activity.GetSystemService (Context.DevicePolicyService);
			manager.WipeData (0);
		}
	}
}