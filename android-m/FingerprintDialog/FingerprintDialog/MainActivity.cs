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

		FingerprintModule fingerprintModule;
		KeyguardManager mKeyguardManager;
		FingerprintAuthenticationDialogFragment mFragment;
		KeyStore mKeyStore;
		KeyGenerator mKeyGenerator;
		Cipher mCipher;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			fingerprintModule = new FingerprintModule(this);
			mKeyguardManager = fingerprintModule.ProvidesKeyguardManager (this);
			mKeyStore = fingerprintModule.ProvidesKeystore ();
			mKeyGenerator = fingerprintModule.ProvidesKeyGenerator ();
			mCipher = fingerprintModule.ProvidesCipher (mKeyStore);

			RequestPermissions (new [] { Manifest.Permission.UseFingerprint }, 0);
		}


		public override void OnRequestPermissionsResult (int requestCode, string[] permissions, int[] state)
		{
			if (requestCode == 0 && state [0] == (int)Android.Content.PM.Permission.Granted) {
				SetContentView (Resource.Layout.activity_main);
				var purchaseButton = (Button)FindViewById (Resource.Id.purchase_button);
				if (!mKeyguardManager.IsKeyguardSecure) {
					// Show a message that the user hasn't set up a fingerprint or lock screen.
					Toast.MakeText (this, "Secure lock screen hasn't set up.\n"
						+ "Go to 'Settings -> Security -> Fingerprint' to set up a fingerprint",
						ToastLength.Long).Show ();
					purchaseButton.Enabled = false;
					return;
				}
				CreateKey ();
				purchaseButton.Click += (object sender, EventArgs e) => {
					// Show the fingerprint dialog. The user has the option to use the fingerprint with
					// crypto, or you can fall back to using a server-side verified password.
					mFragment.SetCryptoObject (new FingerprintManager.CryptoObject (mCipher));
					mFragment.Show (FragmentManager, DIALOG_FRAGMENT_TAG);
				};

				// Set up the crypto object for later. The object will be authenticated by use
				// of the fingerprint.
				InitCipher ();
			}
		}

		void InitCipher ()
		{
			try {
				mKeyStore.Load (null);
				var key = mKeyStore.GetKey (KEY_NAME, null);
				mCipher.Init (CipherMode.EncryptMode, key);
			} catch (KeyPermanentlyInvalidatedException e) {
				// This happens if the lock screen has been disabled or reset after the key was
				// generated, or if a fingerprint got enrolled after the key was generated.
				Toast.MakeText (this, "Keys are invalidated after created. Retry the purchase\n"
				+ e.Message, ToastLength.Long).Show ();
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
			FindViewById (Resource.Id.purchase_button).Visibility = ViewStates.Gone;
			if (withFingerprint) {
				// If the user has authenticated with fingerprint, verify that using cryptography and
				// then show the confirmation message.
				TryEncrypt ();
			} else {
				// Authentication happened with backup password. Just show the confirmation message.
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
				TextView v = (TextView)FindViewById (Resource.Id.encrypted_message);
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
				byte[] encrypted = mCipher.DoFinal (System.Text.Encoding.Default.GetBytes(SECRET_MESSAGE));
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
		void CreateKey ()
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
	}
}

