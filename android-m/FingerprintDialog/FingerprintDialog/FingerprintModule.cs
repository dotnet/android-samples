using System;
using Android.Content;
using Javax.Crypto;
using Java.Security;
using Java.Lang;
using Android.Views.InputMethods;
using Android.App;
using Android.Hardware.Fingerprint;

namespace FingerprintDialog
{
	public class FingerprintModule
	{
		public Context Context { get; set; }

		public FingerprintModule (Context context)
		{
			Context = context;
		}

		public FingerprintManager ProvidesFingerprintManager (Context context)
		{
			return (FingerprintManager)context.GetSystemService ("FingerprintManager");
		}

		public KeyguardManager ProvidesKeyguardManager (Context context)
		{
			return (KeyguardManager)context.GetSystemService ("keyguard");
		}

		public KeyStore ProvidesKeystore ()
		{
			try {
				return KeyStore.GetInstance ("AndroidKeyStore");
			} catch (KeyStoreException e) {
				throw new RuntimeException ("Failed to get an instance of KeyStore", e);
			}
		}

		public KeyGenerator ProvidesKeyGenerator ()
		{
			try {
				return KeyGenerator.GetInstance ("AES", "AndroidKeyStore");
			} catch (NoSuchAlgorithmException e) {
				throw new RuntimeException ("Failed to get an instance of KeyGenerator", e);
			} catch (NoSuchProviderException e) {
				throw new RuntimeException ("Failed to get an instance of KeyGenerator", e);
			}
		}

		public Cipher ProvidesCipher (KeyStore keyStore)
		{
			try {
				return Cipher.GetInstance ("AES/CBC/PKCS7Padding");
			} catch (NoSuchAlgorithmException e) {
				throw new RuntimeException ("Failed to get an instance of Cipher", e);
			} catch (NoSuchPaddingException e) {
				throw new RuntimeException ("Failed to get an instance of Cipher", e);
			}
		}

		public InputMethodManager ProvidesInputMethodManager (Context context)
		{
			return (InputMethodManager)context.GetSystemService (Context.InputMethodService);
		}
	}
}