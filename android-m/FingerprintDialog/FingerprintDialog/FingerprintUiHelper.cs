using System;
using Android.Hardware.Fingerprints;
using Android.OS;
using Android.Widget;
using Android.Runtime;

namespace FingerprintDialog
{
	public class FingerprintUiHelper : FingerprintManager.AuthenticationCallback
	{
		public interface Callback {

			void OnAuthenticated();

			void OnError();
		}

		static readonly long ERROR_TIMEOUT_MILLIS = 1600;
		static readonly long SUCCESS_DELAY_MILLIS = 1300;

		readonly FingerprintManager mFingerprintManager;
		readonly ImageView mIcon;
		readonly TextView mErrorTextView;
		readonly Callback mCallback;
		CancellationSignal mCancellationSignal;

		bool mSelfCancelled;

		/// <summary>
		/// Builder class for {@link FingerprintUiHelper} in which injected fields from Dagger
		/// holds its fields and takes other arguments in the {@link #build} method.
		/// </summary>
		public class FingerprintUiHelperBuilder
		{
			FingerprintManager mFingerPrintManager;

			public FingerprintUiHelperBuilder (FingerprintManager fingerprintManager)
			{
				mFingerPrintManager = fingerprintManager;
			}

			public FingerprintUiHelper Build (ImageView icon, TextView errorTextView, Callback callback)
			{
				return new FingerprintUiHelper (mFingerPrintManager, icon, errorTextView, callback);
			}
		}

		/// <summary>
		/// Constructor for {@link FingerprintUiHelper}. This method is expected to be called from
		/// only the {@link FingerprintUiHelperBuilder} class.
		/// </summary>
		/// <param name="fingerprintManager">Fingerprint manager.</param>
		/// <param name="icon">Icon.</param>
		/// <param name="errorTextView">Error text view.</param>
		/// <param name="callback">Callback.</param>
		FingerprintUiHelper (FingerprintManager fingerprintManager,
			ImageView icon, TextView errorTextView, Callback callback)
		{
			mFingerprintManager = fingerprintManager;
			mIcon = icon;
			mErrorTextView = errorTextView;
			mCallback = callback;
		}

		public bool IsFingerprintAuthAvailable {
			get {
				return mFingerprintManager.IsHardwareDetected
					&& mFingerprintManager.HasEnrolledFingerprints;
			}
		}

		public void StartListening (FingerprintManager.CryptoObject cryptoObject)
		{
			if (!IsFingerprintAuthAvailable)
				return;
			
			mCancellationSignal = new CancellationSignal ();
			mSelfCancelled = false;
			mFingerprintManager.Authenticate (cryptoObject, mCancellationSignal, 0 /* flags */, this, null);
			mIcon.SetImageResource (Resource.Drawable.ic_fp_40px);
		}

		public void StopListening ()
		{
			if (mCancellationSignal != null) {
				mSelfCancelled = true;
				mCancellationSignal.Cancel ();
				mCancellationSignal = null;
			}
		}

		public override void OnAuthenticationError (int errMsgId, Java.Lang.ICharSequence errString)
		{
			if (!mSelfCancelled) {
				ShowError (errString.ToString ());
				mIcon.PostDelayed (() => {
					mCallback.OnError ();
				}, ERROR_TIMEOUT_MILLIS);
			}
		}

		public override void OnAuthenticationHelp (int helpMsgId, Java.Lang.ICharSequence helpString)
		{
			ShowError (helpString.ToString ());
		}

		public override void OnAuthenticationFailed ()
		{
			ShowError (mIcon.Resources.GetString (Resource.String.fingerprint_not_recognized));
		}

		public override void OnAuthenticationSucceeded (FingerprintManager.AuthenticationResult result)
		{
			mErrorTextView.RemoveCallbacks (ResetErrorTextRunnable);
			mIcon.SetImageResource (Resource.Drawable.ic_fingerprint_success);
			mErrorTextView.SetTextColor (mErrorTextView.Resources.GetColor (Resource.Color.success_color, null));
			mErrorTextView.Text = mErrorTextView.Resources.GetString (Resource.String.fingerprint_success);
			mIcon.PostDelayed (() => {
				mCallback.OnAuthenticated ();
			}, SUCCESS_DELAY_MILLIS);
		}

		void ShowError (string error)
		{
			mIcon.SetImageResource (Resource.Drawable.ic_fingerprint_error);
			mErrorTextView.Text = error;
			mErrorTextView.SetTextColor (
				mErrorTextView.Resources.GetColor (Resource.Color.warning_color, null));
			mErrorTextView.RemoveCallbacks (ResetErrorTextRunnable);
			mErrorTextView.PostDelayed (ResetErrorTextRunnable, ERROR_TIMEOUT_MILLIS);
		}

		void ResetErrorTextRunnable ()
		{
			mErrorTextView.SetTextColor (
				mErrorTextView.Resources.GetColor (Resource.Color.hint_color, null));
			mErrorTextView.Text = mErrorTextView.Resources.GetString (Resource.String.fingerprint_hint);
			mIcon.SetImageResource (Resource.Drawable.ic_fp_40px);
		}
	}
}

