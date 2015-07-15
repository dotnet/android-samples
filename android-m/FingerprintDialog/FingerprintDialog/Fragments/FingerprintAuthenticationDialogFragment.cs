
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
using Android.Hardware.Fingerprint;
using Android.Views.InputMethods;
using Android;

namespace FingerprintDialog
{
	/// <summary>
	/// A dialog which uses fingerprint APIs to authenticate the user, and falls back to password
	/// authentication if fingerprint is not available.
	/// </summary>
	public class FingerprintAuthenticationDialogFragment : DialogFragment, 
	TextView.IOnEditorActionListener, FingerprintUiHelper.Callback
	{
		Button mCancelButton;
		Button mSecondDialogButton;
		View mFingerprintContent;
		View mBackupContent;
		EditText mPassword;
		CheckBox mUseFingerprintFutureCheckBox;
		TextView mPasswordDescriptionTextView;
		TextView mNewFingerprintEnrolledTextView;

		Stage mStage = Stage.Fingerprint;

		FingerprintManager.CryptoObject mCryptoObject;
		FingerprintUiHelper mFingerprintUiHelper;

		FingerprintUiHelper.FingerprintUiHelperBuilder mFingerprintUiHelperBuilder;
		InputMethodManager mInputMethodManager;
		ISharedPreferences mSharedPreferences;


		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Create your fragment here
			RetainInstance = true;
			SetStyle (DialogFragmentStyle.Normal, Android.Resource.Style.ThemeMaterialLightDialog);
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			Dialog.SetTitle (GetString (Resource.String.sign_in));
			var v = inflater.Inflate (Resource.Layout.fingerprint_dialog_container, container, false);
			mCancelButton = (Button)v.FindViewById (Resource.Id.cancel_button);
			mCancelButton.Click += (object sender, EventArgs e) => Dismiss ();

			mSecondDialogButton = (Button)v.FindViewById (Resource.Id.second_dialog_button);
			mSecondDialogButton.Click += (object sender, EventArgs e) => {
				if (mStage == Stage.Fingerprint) {
					GoToBackup ();
				} else {
					VerifyPassword ();
				}
			};

			mFingerprintContent = v.FindViewById (Resource.Id.fingerprint_container);
			mBackupContent = v.FindViewById (Resource.Id.backup_container);
			mPassword = v.FindViewById<EditText> (Resource.Id.password);
			mPassword.SetOnEditorActionListener (this);
			mPasswordDescriptionTextView = v.FindViewById<TextView> (Resource.Id.password_description);
			mUseFingerprintFutureCheckBox = v.FindViewById<CheckBox> (Resource.Id.use_fingerprint_in_future_check);
			mNewFingerprintEnrolledTextView = v.FindViewById<TextView> (Resource.Id.new_fingerprint_enrolled_description);
			mFingerprintUiHelper = mFingerprintUiHelperBuilder.Build (
				(ImageView)v.FindViewById (Resource.Id.fingerprint_icon),
				(TextView)v.FindViewById (Resource.Id.fingerprint_status), this);
			UpdateStage ();

			// If fingerprint authentication is not available, switch immediately to the backup
			// (password) screen.
			if (!mFingerprintUiHelper.IsFingerprintAuthAvailable)
				GoToBackup ();

			return v;
		}

		public override void OnResume ()
		{
			base.OnResume ();

			if (mStage == Stage.Fingerprint)
				mFingerprintUiHelper.StartListening (mCryptoObject);
		}

		public void SetStage (Stage stage)
		{
			mStage = stage;
		}

		public override void OnPause ()
		{
			base.OnPause ();

			mFingerprintUiHelper.StopListening ();
		}

		/// <summary>
		/// Sets the crypto object to be passed in when authenticating with fingerprint.
		/// </summary>
		/// <param name="cryptoObject">Crypto object.</param>
		public void SetCryptoObject (FingerprintManager.CryptoObject cryptoObject)
		{
			mCryptoObject = cryptoObject;
		}

		/// <summary>
		/// Switches to backup (password) screen. This either can happen when fingerprint is not
		/// available or the user chooses to use the password authentication method by pressing the
		/// button. This can also happen when the user had too many fingerprint attempts.
		/// </summary>
		void GoToBackup ()
		{
			mStage = Stage.Password;
			UpdateStage ();
			mPassword.RequestFocus ();

			// Show the keyboard.
			mPassword.PostDelayed (ShowKeyboardRunnable, 500);

			// Fingerprint is not used anymore. Stop listening for it.
			mFingerprintUiHelper.StopListening ();
		}

		/// <summary>
		/// Checks whether the current entered password is correct, and dismisses the the dialog and
		/// let's the activity know about the result.
		/// </summary>
		void VerifyPassword ()
		{
			if (CheckPassword (mPassword.Text)) {
				((MainActivity)Activity).OnPurchased (false /* without Fingerprint */);
				Dismiss ();
			} else {
				// assume the password is always correct.
				if (!CheckPassword (mPassword.Text)) {
					return;
				}
				var activity = ((MainActivity)Activity);
				if (mStage == Stage.NewFingerprintEnrolled) {
					var editor = mSharedPreferences.Edit ();
					editor.PutBoolean (GetString (Resource.String.use_fingerprint_to_authenticate_key),
						mUseFingerprintFutureCheckBox.Checked);
					editor.Apply ();

					if (mUseFingerprintFutureCheckBox.Checked) {
						// Re-create the key so that fingerprints including new ones are validated.
						activity.CreateKey ();
						mStage = Stage.Fingerprint;
					}
				}
				mPassword.Text = "";
				((MainActivity)Activity).OnPurchased (false /* without Fingerprint */);
				Dismiss ();
			}
		}

		/// <summary>
		/// Checks the password.
		/// </summary>
		/// <returns><c>true</c>, if {@code password} is correct, <c>false</c> otherwise.</returns>
		/// <param name="password">Password.</param>
		bool CheckPassword (string password)
		{
			// Assume the password is always correct.
			// In the real world situation, the password needs to be verified in the server side.
			return password.Length > 0;
		}

		void ShowKeyboardRunnable ()
		{
			mInputMethodManager.ShowSoftInput (mPassword, 0);
		}

		void UpdateStage ()
		{
			switch (mStage) {
			case Stage.Fingerprint:
				mCancelButton.Text = mCancelButton.Resources.GetString (Resource.String.cancel);
				mSecondDialogButton.Text = mSecondDialogButton.Resources.GetString (Resource.String.use_password);
				mFingerprintContent.Visibility = ViewStates.Visible;
				mBackupContent.Visibility = ViewStates.Gone;
				break;
			case Stage.NewFingerprintEnrolled:
				// Intentional fall through
			case Stage.Password:
				mCancelButton.Text = mCancelButton.Resources.GetString (Resource.String.cancel);
				mSecondDialogButton.Text = mSecondDialogButton.Resources.GetString (Resource.String.ok);
				mFingerprintContent.Visibility = ViewStates.Gone;
				mBackupContent.Visibility = ViewStates.Visible;
				if (mStage == Stage.NewFingerprintEnrolled) {
					mPasswordDescriptionTextView.Visibility = ViewStates.Gone;
					mNewFingerprintEnrolledTextView.Visibility = ViewStates.Visible;
					mUseFingerprintFutureCheckBox.Visibility = ViewStates.Visible;
				}
				break;
			}
		}

		public bool OnEditorAction (TextView v, ImeAction actionId, KeyEvent ev)
		{
			if (actionId == ImeAction.Go) {
				VerifyPassword ();
				return true;
			}
			return false;
		}

		public void OnAuthenticated ()
		{
			// Callback from FingerprintUiHelper. Let the activity know that authentication was
			// successful.
			((MainActivity)Activity).OnPurchased (true /* withFingerprint */);
			Dismiss ();
		}

		public void OnError ()
		{
			GoToBackup ();
		}

		/// <summary>
		/// Enumeration to indicate which authentication method the user is trying to authenticate with.
		/// </summary>
		public enum Stage
		{
			Fingerprint,
			NewFingerprintEnrolled,
			Password
		}
	}
}