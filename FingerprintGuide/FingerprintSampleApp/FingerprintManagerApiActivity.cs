using System;
using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V4.Hardware.Fingerprint;
using Android.Util;
using Android.Views;
using Android.Widget;
using Res = Android.Resource;

namespace Xamarin.FingerprintSample
{
    /// <summary>
    ///     Fingerprint manager API activity.
    /// </summary>
    [Activity(Label = "@string/app_name", MainLauncher = true, Icon = "@mipmap/icon")]
    public class FingerprintManagerApiActivity : Activity
    {
        // ReSharper disable InconsistentNaming
        static readonly string TAG = "X:" + typeof (FingerprintManagerApiActivity).Name;
        static readonly string DIALOG_FRAGMENT_TAG = "fingerprint_auth_fragment";
        static readonly int ERROR_TIMEOUT = 250;
        // ReSharper restore InconsistentNaming
        bool _canScan;

        FingerprintManagerApiDialogFragment _dialogFrag;
        View _errorPanel, _authenticatedPanel, _initialPanel, _scanInProgressPanel;
        TextView _errorTextView1, _errorTextView2;
        FingerprintManagerCompat _fingerprintManager;
        Button _startAuthenticationScanButton, _scanAgainButton, _failedScanAgainButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_fingerprintmanager_api);

            InitializeViewReferences();

            _fingerprintManager = FingerprintManagerCompat.From(this);
            string canScanMsg = CheckFingerprintEligibility();

            _startAuthenticationScanButton.Click += StartFingerprintScan;
            _scanAgainButton.Click += ScanAgainButtonOnClick;
            _failedScanAgainButton.Click += RecheckEligibility;

            if (_canScan)
            {
                _dialogFrag = FingerprintManagerApiDialogFragment.NewInstance(_fingerprintManager);
            }
            else
            {
                ShowError("Can't use this device for the sample.", canScanMsg);
            }
        }

        void RecheckEligibility(object sender, EventArgs eventArgs)
        {
            string canScanMsg = CheckFingerprintEligibility();
            if (_canScan)
            {
                _dialogFrag = FingerprintManagerApiDialogFragment.NewInstance(_fingerprintManager);
                _initialPanel.Visibility = ViewStates.Visible;
                _authenticatedPanel.Visibility = ViewStates.Gone;
                _errorPanel.Visibility = ViewStates.Gone;
                _scanInProgressPanel.Visibility = ViewStates.Gone;
            }
            else
            {
                Log.Debug(TAG, "This device is still ineligiblity for fingerprint authentication. ");
                _dialogFrag = null;
                ShowError("Can't use this device for the sample.", canScanMsg);
            }
        }


        protected override void OnResume()
        {
            base.OnResume();
            Log.Debug(TAG, "OnResume");
        }

        protected override void OnPause()
        {
            base.OnPause();
            Log.Debug(TAG, "OnPause");
        }

        void StartFingerprintScan(object sender, EventArgs args)
        {
            Permission permissionResult = ContextCompat.CheckSelfPermission(this,
                                                                                   Manifest.Permission.UseFingerprint);
            if (permissionResult == Permission.Granted)
            {
                _initialPanel.Visibility = ViewStates.Gone;
                _authenticatedPanel.Visibility = ViewStates.Gone;
                _errorPanel.Visibility = ViewStates.Gone;
                _scanInProgressPanel.Visibility = ViewStates.Visible;
                _dialogFrag.Init();
                _dialogFrag.Show(FragmentManager, DIALOG_FRAGMENT_TAG);
            }
            else
            {
                Snackbar.Make(FindViewById(Res.Id.Content),
                              Resource.String.missing_fingerprint_permissions,
                              Snackbar.LengthLong)
                        .Show();
            }
        }

        void ScanAgainButtonOnClick(object sender, EventArgs eventArgs)
        {
            _initialPanel.Visibility = ViewStates.Visible;
            _authenticatedPanel.Visibility = ViewStates.Gone;
            _errorPanel.Visibility = ViewStates.Gone;
            _scanInProgressPanel.Visibility = ViewStates.Gone;
        }

        void InitializeViewReferences()
        {
            _scanInProgressPanel = FindViewById(Resource.Id.scan_in_progress);
            _initialPanel = FindViewById(Resource.Id.initial_panel);
            _startAuthenticationScanButton = FindViewById<Button>(Resource.Id.start_authentication_scan_buton);

            _errorPanel = FindViewById(Resource.Id.error_panel);
            _errorTextView1 = FindViewById<TextView>(Resource.Id.error_text1);
            _errorTextView2 = FindViewById<TextView>(Resource.Id.error_text2);
            _failedScanAgainButton = FindViewById<Button>(Resource.Id.failed_scan_again_button);

            _authenticatedPanel = FindViewById(Resource.Id.authenticated_panel);
            _scanAgainButton = FindViewById<Button>(Resource.Id.scan_again_button);
        }

        /// <summary>
        ///     Checks to see if the hardware is available to scan for fingerprints
        ///     and that the user has fingerprints enrolled.
        /// </summary>
        /// <returns></returns>
        string CheckFingerprintEligibility()
        {
            _canScan = true;

            if (!_fingerprintManager.IsHardwareDetected)
            {
                _canScan = false;
                string msg = Resources.GetString(Resource.String.missing_fingerprint_scanner);
                Log.Warn(TAG, msg);
                return msg;
            }

            KeyguardManager keyguardManager = (KeyguardManager) GetSystemService(KeyguardService);
            if (!keyguardManager.IsKeyguardSecure)
            {
                string msg = Resources.GetString(Resource.String.keyguard_disabled);
                _canScan = false;
                Log.Warn(TAG, msg);
                return msg;
            }


            if (!_fingerprintManager.HasEnrolledFingerprints)
            {
                _canScan = false;
                string msg = Resources.GetString(Resource.String.register_fingerprint);
                Log.Warn(TAG, msg);
                return msg;
            }

            return string.Empty;
        }

        /// <summary>
        ///     Display error message feedback to the user.
        /// </summary>
        /// <param name="text1"></param>
        /// <param name="text2"></param>
        public void ShowError(string text1, string text2 = "")
        {
            Log.Debug(TAG, "ShowError: '{0}' / '{1}'", text1, text2);
            _errorPanel.PostDelayed(() =>
                                    {
                                        _errorTextView1.Text = text1;
                                        _errorTextView2.Text = text2;

                                        _initialPanel.Visibility = ViewStates.Gone;
                                        _authenticatedPanel.Visibility = ViewStates.Gone;
                                        _errorPanel.Visibility = ViewStates.Visible;
                                        _scanInProgressPanel.Visibility = ViewStates.Gone;
                                    }, ERROR_TIMEOUT);
        }

        public void AuthenticationSuccessful()
        {
            _initialPanel.Visibility = ViewStates.Gone;
            _authenticatedPanel.Visibility = ViewStates.Visible;
            _errorPanel.Visibility = ViewStates.Gone;
            _scanInProgressPanel.Visibility = ViewStates.Gone;
        }
    }
}