using System;

using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Hardware.Fingerprint;
using Android.OS;
using Android.Runtime;
using Android.Security.Keystore;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Security;
using Javax.Crypto;
using Java.Lang;
using Java.Security.Cert;
using Java.IO;

namespace FingerprintDialog
{
	/// <summary>
	/// Main entry point for the sample, showing a backpack and "Purchase" button.
	/// </summary>
	[Activity (MainLauncher = true, Label = "@string/app_name", Theme = "@style/AppTheme")]
	public class MainActivity : Activity
	{
		static readonly string TAG = "MainActivity";

		static readonly string DIALOG_FRAGMENT_TAG = "myFragment";
		static readonly string SECRET_MESSAGE = "Very secret message";

		// Alias for our key in the Android Key Store
		static readonly string KEY_NAME = "my_key";
		static readonly int FINGERPRINT_PERMISSION_REQUEST_CODE = 0;

		FingerprintModule fingerprintModule;
		KeyguardManager mKeyguardManager;
		FingerprintAuthenticationDialogFragment mFragment;
		KeyStore mKeyStore;
		KeyGenerator mKeyGenerator;
		Cipher mCipher;
		ISharedPreferences mSharedPreferences;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			fingerprintModule = new FingerprintModule (this);
			mKeyguardManager = fingerprintModule.ProvidesKeyguardManager (this);
			mKeyStore = fingerprintModule.ProvidesKeystore ();
			mKeyGenerator = fingerprintModule.ProvidesKeyGenerator ();
			mCipher = fingerprintModule.ProvidesCipher (mKeyStore);

			RequestPermissions (new [] { Manifest.Permission.UseFingerprint }, FINGERPRINT_PERMISSION_REQUEST_CODE);

		}


		public override void OnRequestPermissionsResult (int requestCode, string[] permissions, int[] state)
		{
			if (requestCode == FINGERPRINT_PERMISSION_REQUEST_CODE && state [0] == (int)Android.Content.PM.Permission.Granted) {
				SetContentView (Resource.Layout.activity_main);
				var purchaseButton = FindViewById<Button> (Resource.Id.purchase_button);

				if (!mKeyguardManager.IsKeyguardSecure) {
					// Show a message that the user hasn't set up a fingerprint or lock screen.
					Toast.MakeText (this, "Secure lock screen hasn't set up.\n"
					+ "Go to 'Settings -> Security -> Fingerprint' to set up a fingerprint",
						ToastLength.Long).Show ();
					purchaseButton.Enabled = false;
					return;
				}

				if (!CreateKey ()) {
					purchaseButton.Enabled = false;
					return;
				}

				purchaseButton.Enabled = true;
				purchaseButton.Click += (sender, e) => {
					// Show the fingerprint dialog. The user has the option to use the fingerprint with
					// crypto, or you can fall back to using a server-side verified password.
					FindViewById (Resource.Id.confirmation_message).Visibility = ViewStates.Gone;
					FindViewById (Resource.Id.encrypted_message).Visibility = ViewStates.Gone;

					if (InitCipher ()) {
						mFragment.SetCryptoObject (new FingerprintManager.CryptoObject (mCipher));
						var useFingerprintPreference = mSharedPreferences.GetBoolean (GetString (Resource.String.use_fingerprint_to_authenticate_key), true);
						if (useFingerprintPreference) {
							mFragment.SetStage (FingerprintAuthenticationDialogFragment.Stage.Fingerprint);
						} else {
							mFragment.SetStage (FingerprintAuthenticationDialogFragment.Stage.Password);
						}
						mFragment.Show (FragmentManager, DIALOG_FRAGMENT_TAG);
					} else {
						mFragment.SetCryptoObject (new FingerprintManager.CryptoObject (mCipher));
						mFragment.SetStage (FingerprintAuthenticationDialogFragment.Stage.NewFingerprintEnrolled);
						mFragment.Show (FragmentManager, DIALOG_FRAGMENT_TAG);
					}
				};
			}
		}

		bool InitCipher ()
		{
			try {
				mKeyStore.Load (null);
				var key = mKeyStore.GetKey (KEY_NAME, null);
				mCipher.Init (CipherMode.EncryptMode, key);
				return true;
			} catch (KeyPermanentlyInvalidatedException e) {
				return false;
			} catch (KeyStoreException e) {
				throw new RuntimeException ("Failed to init Cipher", e);
			} catch (CertificateException e) {
				throw new RuntimeException ("Failed to init Cipher", e);
			} catch (UnrecoverableKeyException e) {
				throw new RuntimeException ("Failed to init Cipher", e);
			} catch (IOException e) {
				throw new RuntimeException ("Failed to init Cipher", e);
			} catch (NoSuchAlgorithmException e) {
				throw new RuntimeException ("Failed to init Cipher", e);
			} catch (InvalidKeyException e) {
				throw new RuntimeException ("Failed to init Cipher", e);
			}
		}

		public void OnPurchased (bool withFingerprint)
		{
			if (withFingerprint) {
				TryEncrypt ();
			} else {
				ShowConfirmation (null);
			}
		}

		/// <summary>
		/// Show confirmation, if fingerprint was used show crypto information
		/// </summary>
		/// <param name="encrypted">Encrypted.</param>
		void ShowConfirmation (byte[] encrypted)
		{
			FindViewById (Resource.Id.confirmation_message).Visibility = ViewStates.Visible;
			if (encrypted != null) {
				TextView v = FindViewById<TextView> (Resource.Id.encrypted_message);
				v.Visibility = ViewStates.Visible;
				v.Text = Base64.EncodeToString (encrypted, 0 /* flags */);
			}
		}

		/// <summary>
		/// Tries to encrypt some data with the generated key in {@link #createKey} which is only works 
		/// if the user has just authenticated via fingerprint.
		/// </summary>
		void TryEncrypt ()
		{
			try {
				byte[] encrypted = mCipher.DoFinal (System.Text.Encoding.Default.GetBytes (SECRET_MESSAGE));
				ShowConfirmation (encrypted);
			} catch (BadPaddingException e) {
				Toast.MakeText (this, "Failed to encrypt the data with the generated key. "
				+ "Retry the purchase", ToastLength.Long).Show ();
				Log.Error (TAG, "Failed to encrypt the data with the generated key." + e.Message);
			} catch (IllegalBlockSizeException e) {
				Toast.MakeText (this, "Failed to encrypt the data with the generated key. "
				+ "Retry the purchase", ToastLength.Long).Show ();
				Log.Error (TAG, "Failed to encrypt the data with the generated key." + e.Message);
			}
		}


		/// <summary>
		/// Creates a symmetric key in the Android Key Store which can only be used after the user 
		/// has authenticated with fingerprint.
		/// </summary>
		public bool CreateKey ()
		{
			// The enrolling flow for fingerprint. This is where you ask the user to set up fingerprint
			// for your flow. Use of keys is necessary if you need to know if the set of
			// enrolled fingerprints has changed.
			try {
				mKeyStore.Load (null);
				// Set the alias of the entry in Android KeyStore where the key will appear
				// and the constrains (purposes) in the constructor of the Builder
				mKeyGenerator.Init (new KeyGenParameterSpec.Builder (KEY_NAME,
					KeyProperties.PurposeEncrypt |
					KeyProperties.PurposeDecrypt)
					.SetBlockModes (KeyProperties.BlockModeCbc)
				// Require the user to authenticate with a fingerprint to authorize every use
				// of the key
					.SetUserAuthenticationRequired (true)
					.SetEncryptionPaddings (KeyProperties.EncryptionPaddingPkcs7)
					.Build ());
				mKeyGenerator.GenerateKey ();
				return true;
			} catch (IllegalStateException e) {
				Toast.MakeText (this, "Go to 'Settings -> Security -> Fingerprint' and register at least one fingerprint", ToastLength.Long).Show ();
				return false;
			} catch (NoSuchAlgorithmException e) {
				throw new RuntimeException (e);
			} catch (InvalidAlgorithmParameterException e) {
				throw new RuntimeException (e);
			} catch (CertificateException e) {
				throw new RuntimeException (e);
			} catch (IOException e) {
				throw new RuntimeException (e);
			}
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.menu_main, menu);
			return true;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			var id = item.ItemId;

			if (id == Resource.Id.action_settings) {
				var intent = new Intent(this, typeof(SettingsActivity));
				StartActivity(intent);
				return true;
			}
			return base.OnOptionsItemSelected(item);
		}
	}
}

