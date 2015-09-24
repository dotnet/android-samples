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
using Android.Support.V4.App;
using Android.Support.Design.Widget;

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
 	* directly to it. Permissions are verified and requested through compat helpers in the support v4
 	* library, in this Activity using {@link ActivityCompat}.
 	* First, permissions are checked if they have already been granted through {@link
 	* ActivityCompat#checkSelfPermission(Context, String)}.
 	* If permissions have not been granted, they are requested through
 	* {@link ActivityCompat#requestPermissions(Activity, String[], int)} and the return value checked
 	* in
 	* a callback to the {@link android.support.v4.app.ActivityCompat.OnRequestPermissionsResultCallback}
 	* interface.
 	* <p>
 	* Before requesting permissions, {@link ActivityCompat#shouldShowRequestPermissionRationale(Activity,
 	* String)}
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
	[Activity (Label = "@string/app_name", MainLauncher = true, Theme = "@style/Theme.AppCompat.Light")]
	public class MainActivity : SampleActivityBase, ActivityCompat.IOnRequestPermissionsResultCallback 
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
     	* Root of the layout of this Activity.
     	*/
		View layout;

		/**
     	* Called when the 'show camera' button is clicked.
     	* Callback is defined in resource layout definition.
     	*/
		[Export]
		public void ShowCamera (View view)
		{
			Log.Info (TAG, "Show camera button pressed. Checking permission.");

			// Check if the Camera permission is already available.
			if (ActivityCompat.CheckSelfPermission (this, Manifest.Permission.Camera) != (int)Permission.Granted) {
				
				// Camera permission has not been granted
				RequestCameraPermission ();
			} else {
				// Camera permissions is already available, show the camera preview.
				Log.Info (TAG, "CAMERA permission has already been granted. Displaying camera preview.");
				ShowCameraPreview ();
			}
		}
				
		/**
     	* Requests the Camera permission.
		* If the permission has been denied previously, a SnackBar will prompt the user to grant the
		* permission, otherwise it is requested directly.
		*/
		void RequestCameraPermission ()
		{
			Log.Info (TAG, "CAMERA permission has NOT been granted. Requesting permission.");

			if (ActivityCompat.ShouldShowRequestPermissionRationale (this, Manifest.Permission.Camera)) {
				// Provide an additional rationale to the user if the permission was not granted
				// and the user would benefit from additional context for the use of the permission.
				// For example if the user has previously denied the permission.
				Log.Info (TAG, "Displaying camera permission rationale to provide additional context.");

				Snackbar.Make (layout, Resource.String.permission_camera_rationale,
					Snackbar.LengthIndefinite).SetAction (Resource.String.ok, new Action<View> (delegate(View obj) {
						ActivityCompat.RequestPermissions (this, new String[] { Manifest.Permission.Camera }, REQUEST_CAMERA);
					})).Show ();
			} else {
				// Camera permission has not been granted yet. Request it directly.
				ActivityCompat.RequestPermissions (this, new String[] { Manifest.Permission.Camera }, REQUEST_CAMERA);
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
			if (ActivityCompat.CheckSelfPermission (this, Manifest.Permission.ReadContacts) != (int)Permission.Granted
				|| ActivityCompat.CheckSelfPermission (this, Manifest.Permission.WriteContacts) != (int)Permission.Granted) {
				// Contacts permissions have not been granted.
				Log.Info (TAG, "Contact permissions has NOT been granted. Requesting permissions.");
				RequestContactsPermissions ();
			} else {
				// Contact permissions have been granted. Show the contacts fragment.
				Log.Info (TAG, "Contact permissions have already been granted. Displaying contact details.");
				ShowContactDetails ();
			}
		}

		/**
     	* Requests the Contacts permissions.
     	* If the permission has been denied previously, a SnackBar will prompt the user to grant the
     	* permission, otherwise it is requested directly.
     	*/
		void RequestContactsPermissions ()
		{
			if (ActivityCompat.ShouldShowRequestPermissionRationale (this, Manifest.Permission.ReadContacts)
				|| ActivityCompat.ShouldShowRequestPermissionRationale (this, Manifest.Permission.WriteContacts)) {

				// Provide an additional rationale to the user if the permission was not granted
				// and the user would benefit from additional context for the use of the permission.
				// For example, if the request has been denied previously.
				Log.Info (TAG, "Displaying contacts permission rationale to provide additional context.");

				// Display a SnackBar with an explanation and a button to trigger the request.
				Snackbar.Make (layout, Resource.String.permission_contacts_rationale,
					Snackbar.LengthIndefinite).SetAction (Resource.String.ok, new Action<View> (delegate(View obj) {
						ActivityCompat.RequestPermissions (this, PERMISSIONS_CONTACT, REQUEST_CONTACTS);
					})).Show ();
			} else {
				// Contact permissions have not been granted yet. Request them directly.
				ActivityCompat.RequestPermissions (this, PERMISSIONS_CONTACT, REQUEST_CONTACTS);
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
		public override void OnRequestPermissionsResult (int requestCode, string[] permissions, Permission[] grantResults)
		{
			if (requestCode == REQUEST_CAMERA) {
				// Received permission result for camera permission.
				Log.Info (TAG, "Received response for Camera permission request.");

				// Check if the only required permission has been granted
				if (grantResults.Length == 1 && grantResults[0] == Permission.Granted) {
					// Camera permission has been granted, preview can be displayed
					Log.Info (TAG, "CAMERA permission has now been granted. Showing preview.");
					Snackbar.Make (layout, Resource.String.permision_available_camera, Snackbar.LengthShort).Show ();
				} else {
					Log.Info (TAG, "CAMERA permission was NOT granted.");
					Snackbar.Make (layout, Resource.String.permissions_not_granted, Snackbar.LengthShort).Show ();
				}
			} else if (requestCode == REQUEST_CONTACTS) {
				Log.Info (TAG, "Received response for contact permissions request.");

				// We have requested multiple permissions for contacts, so all of them need to be
				// checked.
				if (PermissionUtil.VerifyPermissions (grantResults)) {
					// All required permissions have been granted, display contacts fragment.
					Snackbar.Make (layout, Resource.String.permision_available_contacts, Snackbar.LengthShort).Show ();
				} else {
					Log.Info (TAG, "Contacts permissions were NOT granted.");
					Snackbar.Make (layout, Resource.String.permissions_not_granted, Snackbar.LengthShort).Show ();
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
			layout = FindViewById (Resource.Id.sample_main_layout);

			if (bundle == null) {
				Android.App.FragmentTransaction transaction = FragmentManager.BeginTransaction ();
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


