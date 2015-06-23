using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Security.Keystore;

using Java.Security;
using Javax.Crypto;

namespace ConfirmCredential
{
	/**
 	* Main entry point for the sample, showing a backpack and "Purchase" button.
 	*/
	[Activity (Label = "@string/app_name", MainLauncher = true, Theme = "@style/AppTheme")]
	public class MainActivity : Activity
	{
		static readonly string KEY_NAME = "my_key";
		static readonly byte[] SECRET_BYTE_ARRAY = new byte[] { 
			1, 2, 3, 4, 5, 6 
		};

		static readonly int REQUEST_CODE_CONFIRM_DEVICE_CREDENTIALS = 1;

		/**
     	* If the user has unlocked the device Within the last this number of seconds,
     	* it can be considered as an authenticator.
     	*/
		static readonly int AUTHENTICATION_DURATION_SECONDS = 10;

		KeyguardManager keyguardManager;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);

			keyguardManager = (KeyguardManager)GetSystemService (Context.KeyguardService);
			var purchaseButton = FindViewById <Button> (Resource.Id.purchase_button);

			if (!keyguardManager.IsKeyguardSecure) {
				// Show a message that the user hasn't set up a lock screen.
				Toast.MakeText (this, "Secure lock screen isn't set up.\n"
					+ "Go to 'Settings -> Security -> Screenlock' to set up a lock screen",
					ToastLength.Short).Show ();
				purchaseButton.Enabled = false;
				return;
			}

			CreateKey ();

			// Test to encrypt something. It might fail if the timeout expired (30s).
			purchaseButton.Click += (object sender, EventArgs e) => TryEncrypt ();
		}

		/**
     	* Tries to encrypt some data with the generated key in CreateKey which
     	* only works if the user has just authenticated via device credentials.
     	*/
		void TryEncrypt ()
		{
			try {
				var keyStore = KeyStore.GetInstance ("AndroidKeyStore");
				keyStore.Load (null);
				IKey secretKey = keyStore.GetKey (KEY_NAME, null);
				var cipher = Cipher.GetInstance (
					KeyProperties.KeyAlgorithmAes + "/" + KeyProperties.BlockModeCbc + "/"
					+ KeyProperties.EncryptionPaddingPkcs7);

				// Try encrypting something, it will only work if the user authenticated within
				// the last AUTHENTICATION_DURATION_SECONDS seconds.
				cipher.Init (CipherMode.EncryptMode, (IKey)secretKey);
				cipher.DoFinal (SECRET_BYTE_ARRAY);

				// If the user has recently authenticated, you will reach here.
				ShowAlreadyAuthenticated ();
			} catch (UserNotAuthenticatedException) {
				// User is not authenticated, let's authenticate with device credentials.
				ShowAuthenticationScreen ();
			} catch (KeyPermanentlyInvalidatedException e) {
				// This happens if the lock screen has been disabled or reset after the key was
				// generated after the key was generated.
				Toast.MakeText (this, "Keys are invalidated after created. Retry the purchase\n"
					+ e.Message, ToastLength.Short).Show ();
			} catch (Exception e) {
				throw new SystemException ("Exception", e);
			}
		}

		/**
     	* Creates a symmetric key in the Android Key Store which can only be used after the user has
     	* authenticated with device credentials within the last X seconds.
     	*/
		void CreateKey ()
		{
			// Generate a key to decrypt payment credentials, tokens, etc.
			// This will most likely be a registration step for the user when they are setting up your app.
			try {
				var keyStore = KeyStore.GetInstance ("AndroidKeyStore");
				keyStore.Load (null);
				var keyGenerator = KeyGenerator.GetInstance (KeyProperties.KeyAlgorithmAes, "AndroidKeyStore");

				// Set the alias of the entry in Android KeyStore where the key will appear
				// and the constrains (purposes) in the constructor of the Builder
				keyGenerator.Init (new KeyGenParameterSpec.Builder (KEY_NAME,
					KeyProperties.PurposeEncrypt | KeyProperties.PurposeDecrypt)
					.SetBlockModes (KeyProperties.BlockModeCbc)
					.SetUserAuthenticationRequired (true)
					// Require that the user has unlocked in the last 30 seconds
					.SetUserAuthenticationValidityDurationSeconds (AUTHENTICATION_DURATION_SECONDS)
					.SetEncryptionPaddings (KeyProperties.EncryptionPaddingPkcs7)
					.Build ());
				keyGenerator.GenerateKey ();
			} catch (Exception e) {
				throw new SystemException ("Failed to create a symmetric key", e);
			}
		}

		void ShowAuthenticationScreen ()
		{
			// Create the Confirm Credentials screen. You can customize the title and description. Or
			// we will provide a generic one for you if you leave it null
			Intent intent = keyguardManager.CreateConfirmDeviceCredentialIntent ((string)null, (string)null);
			if (intent != null) {
				StartActivityForResult (intent, REQUEST_CODE_CONFIRM_DEVICE_CREDENTIALS);
			}
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			if (requestCode == REQUEST_CODE_CONFIRM_DEVICE_CREDENTIALS) {
				// Challenge completed, proceed with using cipher
				if (resultCode == Result.Ok) {
					ShowPurchaseConfirmation ();
				} else {
					// The user canceled or didn’t complete the lock screen
					// operation. Go to error/cancellation flow.
				}
			}
		}

		void ShowPurchaseConfirmation ()
		{
			FindViewById (Resource.Id.confirmation_message).Visibility = ViewStates.Visible;
			FindViewById (Resource.Id.purchase_button).Enabled = false;
		}

		void ShowAlreadyAuthenticated ()
		{
			var textView = FindViewById <TextView> (
				Resource.Id.already_has_valid_device_credential_message);
			textView.Visibility = ViewStates.Visible;

			textView.Text = GetString (
				Resource.String.already_confirmed_device_credentials_within_last_x_seconds,
				AUTHENTICATION_DURATION_SECONDS);
			FindViewById (Resource.Id.purchase_button).Enabled = false;
		}
	}
}


