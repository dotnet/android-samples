using System;
using Android.App;
using Android.Hardware.Fingerprints;
using Android.OS;
using Android.Support.V4.Hardware.Fingerprint;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Javax.Crypto;
using CancellationSignal = Android.Support.V4.OS.CancellationSignal;
using Res = Android.Resource;

// ReSharper disable InconsistentNaming
// ReSharper disable UseStringInterpolation
// ReSharper disable UseNullPropagation

namespace Xamarin.FingerprintSample
{
    /// <summary>
    ///     This DialogFragment is displayed when the app is scanning for fingerprints.
    /// </summary>
    /// <remarks>
    ///     This DialogFragment doesn't perform any checks to see if the device
    ///     is actually eligible for fingerprint authentication. All of those checks are performed by the
    ///     Activity.
    /// </remarks>
    public class FingerprintManagerApiDialogFragment : DialogFragment
    {
        static readonly string TAG = "X:" + typeof (FingerprintManagerApiDialogFragment).Name;

        Button _cancelButton;
        CancellationSignal _cancellationSignal;
        FingerprintManagerCompat _fingerprintManager;

        bool ScanForFingerprintsInOnResume { get; set; } = true;

        bool UserCancelledScan { get; set; }

        CryptoObjectHelper CryptObjectHelper { get; set; }

        bool IsScanningForFingerprints
        {
            // ReSharper disable once ConvertPropertyToExpressionBody
            get { return _cancellationSignal != null; }
        }

        public static FingerprintManagerApiDialogFragment NewInstance(FingerprintManagerCompat fingerprintManager)
        {
            FingerprintManagerApiDialogFragment frag = new FingerprintManagerApiDialogFragment
                                                       {
                                                           _fingerprintManager = fingerprintManager
                                                       };
            return frag;
        }

        public void Init(bool startScanning = true)
        {
            ScanForFingerprintsInOnResume = startScanning;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
            CryptObjectHelper = new CryptoObjectHelper();
            SetStyle(DialogFragmentStyle.Normal, Res.Style.ThemeMaterialLightDialog);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Dialog.SetTitle(Resource.String.sign_in);

            View v = inflater.Inflate(Resource.Layout.dialog_scanning_for_fingerprint, container, false);

            _cancelButton = v.FindViewById<Button>(Resource.Id.cancel_button);
            _cancelButton.Click += (sender, args) =>
                                   {
                                       UserCancelledScan = true;
                                       StopListeningForFingerprints();
                                   };

            return v;
        }

        public override void OnResume()
        {
            base.OnResume();
            if (!ScanForFingerprintsInOnResume)
            {
                return;
            }

            UserCancelledScan = false;
            _cancellationSignal = new CancellationSignal();
            _fingerprintManager.Authenticate(CryptObjectHelper.BuildCryptoObject(),
                                             (int) FingerprintAuthenticationFlags.None, /* flags */
                                             _cancellationSignal,
                                             new SimpleAuthCallbacks(this),
                                             null);
        }

        public override void OnPause()
        {
            base.OnPause();
            if (IsScanningForFingerprints)
            {
                StopListeningForFingerprints(true);
            }
        }

        void StopListeningForFingerprints(bool butStartListeningAgainInOnResume = false)
        {
            if (_cancellationSignal != null)
            {
                _cancellationSignal.Cancel();
                _cancellationSignal = null;
                Log.Debug(TAG, "StopListeningForFingerprints: _cancellationSignal.Cancel();");
            }
            ScanForFingerprintsInOnResume = butStartListeningAgainInOnResume;
        }

        public override void OnDestroyView()
        {
            // see https://code.google.com/p/android/issues/detail?id=17423
            if (Dialog != null && RetainInstance)
            {
                Dialog.SetDismissMessage(null);
            }
            base.OnDestroyView();
        }

        class SimpleAuthCallbacks : FingerprintManagerCompat.AuthenticationCallback
        {
            // ReSharper disable once MemberHidesStaticFromOuterClass
            static readonly string TAG = "X:" + typeof (SimpleAuthCallbacks).Name;
            static readonly byte[] SECRET_BYTES = {1, 2, 3, 4, 5, 6, 7, 8, 9};
            readonly FingerprintManagerApiDialogFragment _fragment;

            public SimpleAuthCallbacks(FingerprintManagerApiDialogFragment frag)
            {
                _fragment = frag;
            }

            public override void OnAuthenticationSucceeded(FingerprintManagerCompat.AuthenticationResult result)
            {
                Log.Debug(TAG, "OnAuthenticationSucceeded");
                if (result.CryptoObject.Cipher != null)
                {
                    try
                    {
                        // Calling DoFinal on the Cipher ensures that the encryption worked.
                        byte[] doFinalResult = result.CryptoObject.Cipher.DoFinal(SECRET_BYTES);
                        Log.Debug(TAG, "Fingerprint authentication succeeded, doFinal results: {0}",
                                  Convert.ToBase64String(doFinalResult));

                        ReportSuccess();
                    }
                    catch (BadPaddingException bpe)
                    {
                        Log.Error(TAG, "Failed to encrypt the data with the generated key." + bpe);
                        ReportAuthenticationFailed();
                    }
                    catch (IllegalBlockSizeException ibse)
                    {
                        Log.Error(TAG, "Failed to encrypt the data with the generated key." + ibse);
                        ReportAuthenticationFailed();
                    }
                }
                else
                {
                    // No cipher used, assume that everything went well and trust the results.
                    Log.Debug(TAG, "Fingerprint authentication succeeded.");
                    ReportSuccess();
                }
            }

            void ReportSuccess()
            {
                FingerprintManagerApiActivity activity = (FingerprintManagerApiActivity) _fragment.Activity;
                activity.AuthenticationSuccessful();
                _fragment.Dismiss();
            }

            void ReportScanFailure(int errMsgId, string errorMessage)
            {
                FingerprintManagerApiActivity activity = (FingerprintManagerApiActivity) _fragment.Activity;
                activity.ShowError(errorMessage, string.Format("Error message id {0}.", errMsgId));
                _fragment.Dismiss();
            }

            void ReportAuthenticationFailed()
            {
                FingerprintManagerApiActivity activity = (FingerprintManagerApiActivity) _fragment.Activity;
                string msg = _fragment.Resources.GetString(Resource.String.authentication_failed_message);
                activity.ShowError(msg);
                _fragment.Dismiss();
            }

            public override void OnAuthenticationError(int errMsgId, ICharSequence errString)
            {
                // There are some situations where we don't care about the error. For example, 
                // if the user cancelled the scan, this will raise errorID #5. We don't want to
                // report that, we'll just ignore it as that event is a part of the workflow.
                bool reportError = (errMsgId == (int) FingerprintState.ErrorCanceled) &&
                                   !_fragment.ScanForFingerprintsInOnResume;

                string debugMsg = string.Format("OnAuthenticationError: {0}:`{1}`.", errMsgId, errString);

                if (_fragment.UserCancelledScan)
                {
                    string msg = _fragment.Resources.GetString(Resource.String.scan_cancelled_by_user);
                    ReportScanFailure(-1, msg);
                }
                else if (reportError)
                {
                    ReportScanFailure(errMsgId, errString.ToString());
                    debugMsg += " Reporting the error.";
                }
                else
                {
                    debugMsg += " Ignoring the error.";
                }
                Log.Debug(TAG, debugMsg);
            }

            public override void OnAuthenticationFailed()
            {
                Log.Info(TAG, "Authentication failed.");
                ReportAuthenticationFailed();
            }

            public override void OnAuthenticationHelp(int helpMsgId, ICharSequence helpString)
            {
                Log.Debug(TAG, "OnAuthenticationHelp: {0}:`{1}`", helpString, helpMsgId);
                ReportScanFailure(helpMsgId, helpString.ToString());
            }
        }
    }
}