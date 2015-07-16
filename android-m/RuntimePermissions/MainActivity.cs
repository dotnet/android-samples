using System;

using Android;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using Android.Util;

using CommonSampleLibrary;
using Log = CommonSampleLibrary.Log;
using Java.Interop;

namespace RuntimePermissions
{
	/**
 	* Launcher Activity that demonstrates the use of runtime permissions for Android M.
 	* It contains a summary sample description, sample log and a Fragment that calls callbacks on this
 	* Activity to illustrate parts of the runtime permissions API.
 	* <p>
 	* This Activity requests permissions to access the camera ({@link android.Manifest.permission#CAMERA})
 	* when the 'Show Camera' button is clicked to display the camera preview.
 	* Contacts permissions (({@link android.Manifest.permission#READ_CONTACTS} and ({@link
 	* android.Manifest.permission#WRITE_CONTACTS})) are requested when the 'Show and Add Contacts'
 	* button is
 	* clicked to display the first contact in the contacts database and to add a dummy contact
 	* directly
 	* to it. First, permissions are checked if they have already been granted through {@link
 	* android.app.Activity#checkSelfPermission(String)} (wrapped in {@link
 	* PermissionUtil#hasSelfPermission(Activity, String)} and {@link PermissionUtil#hasSelfPermission(Activity,
 	* String[])} for compatibility). If permissions have not been granted, they are requested through
 	* {@link Activity#requestPermissions(String[], int)} and the return value checked in {@link
 	* Activity#onRequestPermissionsResult(int, String[], int[])}.
 	* <p>
 	* Before requesting permissions, {@link Activity#shouldShowRequestPermissionRationale(String)}
 	* should be called to provide the user with additional context for the use of permissions if they
 	* have been denied previously.
 	* <p>
 	* If this sample is executed on a device running a platform version below M, all permissions
 	* declared
 	* in the Android manifest file are always granted at install time and cannot be requested at run
 	* time.
 	* <p>
 	* This sample targets the M platform and must therefore request permissions at runtime. Change the
 	* targetSdk in the file 'Application/build.gradle' to 22 to run the application in compatibility
 	* mode.
 	* Now, if a permission has been disable by the system through the application settings, disabled
 	* APIs provide compatibility data.
 	* For example the camera cannot be opened or an empty list of contacts is returned. No special
 	* action is required in this case.
 	* <p>
 	* (This class is based on the MainActivity used in the SimpleFragment sample template.)
 	*/
	[Activity (Label = "@string/app_name", MainLauncher = true, Theme = "@style/AppTheme")]
	public class MainActivity : SampleActivityBase
	{
		public override string TAG {
			get {
				return "MainActivity";
			}
		}

		/**
     	* Id to identify a camera permission request.
     	*/
		static readonly int REQUEST_CAMERA = 0;

		/**
     	* Id to identify a contacts permission request.
     	*/
		static readonly int REQUEST_CONTACTS = 1;

		/**
     	* Permissions required to read and write contacts. Used by the ContactsFragment.
     	*/
		static string[] PERMISSIONS_CONTACT = {
			Manifest.Permission.ReadContacts,
			Manifest.Permission.WriteContacts
		};

		// Whether the Log Fragment is currently shown.
		bool logShown;

		/**
     	* Called when the 'show camera' button is clicked.
     	* Callback is defined in resource layout definition.
     	*/
		[Export]
		public void ShowCamera (View view)
		{
			Log.Info (TAG, "Show camera button pressed. Checking permission.");

			// Check if the Camera permission is already available.
			if (PermissionUtil.HasSelfPermission (this, Manifest.Permission.Camera)) {
				// Camera permissions is already available, show the camera preview.
				Log.Info (TAG, "CAMERA permission has already been granted. Displaying camera preview.");
				ShowCameraPreview ();
			} else {
				// Camera permission has not been granted.
				Log.Info (TAG, "CAMERA permission has NOT been granted. Requesting permission.");

				// Provide an additional rationale to the user if the permission was not granted
				// and the user would benefit from additional context for the use of the permission.
				if (ShouldShowRequestPermissionRationale (Manifest.Permission.Camera)) {
					Log.Info (TAG, "Displaying camera permission rationale to provide additional context.");
					Toast.MakeText (this, Resource.String.permission_camera_rationale, ToastLength.Short).Show ();
				}

				// Request Camera permission.
				RequestPermissions (new string[] { Manifest.Permission.Camera }, REQUEST_CAMERA);
			}
		}

		/**
     	* Called when the 'show camera' button is clicked.
     	* Callback is defined in resource layout definition.
     	*/
		[Export]
		public void ShowContacts (View v)
		{
			Log.Info (TAG, "Show contacts button pressed. Checking permissions.");

			// Verify that all required contact permissions have been granted.
			if (PermissionUtil.HasSelfPermission (this, PERMISSIONS_CONTACT)) {
				// Contact permissions have been granted. Show the contacts fragment.
				Log.Info (TAG, "Contact permissions have already been granted. Displaying contact details.");
				ShowContactDetails ();
			} else {
				// Contacts permissions have not been granted.
				Log.Info (TAG, "Contact permissions has NOT been granted. Requesting permission.");

				// Provide an additional rationale to the user if the permission was not granted
				// and the user would benefit from additional context for the use of the permission.
				if (ShouldShowRequestPermissionRationale (Manifest.Permission.ReadContacts)
					|| ShouldShowRequestPermissionRationale (Manifest.Permission.WriteContacts)) {
					Log.Info (TAG, "Displaying contacts permission rationale to provide additional context.");
					Toast.MakeText (this, Resource.String.permission_contacts_rationale, ToastLength.Short).Show ();
				}

				// Request Contact permission.
				RequestPermissions (PERMISSIONS_CONTACT, REQUEST_CONTACTS);
			}
		}

		/**
		* Display the CameraPreviewFragment in the content area if the required Camera
		* permission has been granted.
		*/
		void ShowCameraPreview ()
		{
			FragmentManager.BeginTransaction ()
				.Replace (Resource.Id.sample_content_fragment, CameraPreviewFragment.NewInstance ())
				.AddToBackStack ("contacts")
				.Commit ();
		}

		/**
     	* Display the ContactsFragment in the content area if the required contacts
     	* permissions have been granted.
     	*/
		void ShowContactDetails ()
		{
			FragmentManager.BeginTransaction ()
				.Replace (Resource.Id.sample_content_fragment, ContactsFragment.NewInstance ())
				.AddToBackStack ("contacts")
				.Commit ();
		}

		/**
     	* Callback received when a permissions request has been completed.
     	*/
		public override void OnRequestPermissionsResult (int requestCode, string[] permissions, int[] grantResults)
		{
			if (requestCode == REQUEST_CAMERA) {
				// Received permission result for camera permission.
				Log.Info (TAG, "Received response for Camera permission request.");

				// Check if the only required permission has been granted
				if (grantResults[0] == (int)Permission.Granted) {
					// Camera permission has been granted, preview can be displayed
					Log.Info (TAG, "CAMERA permission has now been granted. Showing preview.");
					Toast.MakeText (this, Resource.String.permision_available_camera, ToastLength.Short).Show ();
				} else {
					Log.Info (TAG, "CAMERA permission was NOT granted.");
					Toast.MakeText (this, Resource.String.permissions_not_granted, ToastLength.Short).Show ();
				}
			} else if (requestCode == REQUEST_CONTACTS) {
				Log.Info (TAG, "Received response for contact permissions request.");

				// We have requested multiple permissions for contacts, so all of them need to be
				// checked.
				if (PermissionUtil.VerifyPermissions (grantResults)) {
					// All required permissions have been granted, display contacts fragment.
					Toast.MakeText (this, Resource.String.permision_available_contacts, ToastLength.Short).Show ();
				} else {
					Log.Info (TAG, "Contacts permissions were NOT granted.");
					Toast.MakeText (this, Resource.String.permissions_not_granted, ToastLength.Short).Show ();
				}

			} else {
				base.OnRequestPermissionsResult (requestCode, permissions, grantResults);
			}
		}

		/* Note: Methods and definitions below are only used to provide the UI for this sample and are
    	not relevant for the execution of the runtime permissions API. */

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.main, menu);
			return true;
		}

		public override bool OnPrepareOptionsMenu (IMenu menu)
		{
			IMenuItem logToggle = menu.FindItem (Resource.Id.menu_toggle_log);
			logToggle.SetVisible (FindViewById (Resource.Id.sample_output) is ViewAnimator);
			logToggle.SetTitle (logShown ? Resource.String.sample_hide_log : Resource.String.sample_show_log);

			return base.OnPrepareOptionsMenu (menu);
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case Resource.Id.menu_toggle_log:
				logShown = !logShown;
				var output = FindViewById <ViewAnimator> (Resource.Id.sample_output);

				if (logShown)
					output.DisplayedChild = 1;
				else
					output.DisplayedChild = 0;

				InvalidateOptionsMenu ();
				return true;
			}
			return base.OnOptionsItemSelected (item);
		}

		public override void InitializeLogging ()
		{
			// Wraps Android's native log framework.
			var logWrapper = new LogWrapper ();
			// Using Log, front-end to the logging chain, emulates android.util.log method signatures.
			Log.LogNode = logWrapper;

			// Filter strips out everything except the message text.
			var msgFilter = new MessageOnlyLogFilter ();
			logWrapper.NextNode = msgFilter;

			// On screen logging via a fragment with a TextView.
			var logFragment = (LogFragment)FragmentManager.FindFragmentById (Resource.Id.log_fragment);
			msgFilter.NextNode = logFragment.LogView;
		}

		[Export]
		public void OnBackClick (View view)
		{
			FragmentManager.PopBackStack ();
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.activity_main);

			if (bundle == null) {
				FragmentTransaction transaction = FragmentManager.BeginTransaction ();
				var fragment = new RuntimePermissionsFragment ();
				transaction.Replace (Resource.Id.sample_content_fragment, fragment);
				transaction.Commit ();
			}

			// This method sets up our custom logger, which will print all log messages to the device
			// screen, as well as to adb logcat.
			InitializeLogging ();
		}

	}
}


