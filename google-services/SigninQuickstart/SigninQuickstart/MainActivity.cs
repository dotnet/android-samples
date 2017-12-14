using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Gms.Common.Apis;
using Android.Support.V7.App;
using Android.Gms.Common;
using Android.Util;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Auth.Api;

namespace SigninQuickstart
{
	[Activity (MainLauncher = true, Theme = "@style/ThemeOverlay.MyNoTitleActivity")]
	[Register("com.xamarin.signinquickstart.MainActivity")]
	public class MainActivity : AppCompatActivity, View.IOnClickListener, GoogleApiClient.IOnConnectionFailedListener
	{
		const string TAG = "MainActivity";

		const int RC_SIGN_IN = 9001;

		GoogleApiClient mGoogleApiClient;
		TextView mStatusTextView;
		ProgressDialog mProgressDialog;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.activity_main);

			mStatusTextView = FindViewById<TextView>(Resource.Id.status);
			FindViewById(Resource.Id.sign_in_button).SetOnClickListener(this);
			FindViewById(Resource.Id.sign_out_button).SetOnClickListener(this);
			FindViewById(Resource.Id.disconnect_button).SetOnClickListener(this);

			// [START configure_signin]
			// Configure sign-in to request the user's ID, email address, and basic
			// profile. ID and basic profile are included in DEFAULT_SIGN_IN.
			GoogleSignInOptions gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
					.RequestEmail()
					.Build();
			// [END configure_signin]

			// [START build_client]
			// Build a GoogleApiClient with access to the Google Sign-In API and the
			// options specified by gso.
			mGoogleApiClient = new GoogleApiClient.Builder(this)
					.EnableAutoManage(this /* FragmentActivity */, this /* OnConnectionFailedListener */)
			        .AddApi(Auth.GOOGLE_SIGN_IN_API, gso)
					.Build();
			// [END build_client]

			// [START customize_button]
			// Set the dimensions of the sign-in button.
			var signInButton = FindViewById<SignInButton>(Resource.Id.sign_in_button);
			signInButton.SetSize(SignInButton.SizeStandard);
			// [END customize_button]
		}

		protected override void OnStart()
		{
			base.OnStart();

			var opr = Auth.GoogleSignInApi.SilentSignIn(mGoogleApiClient);
			if (opr.IsDone)
			{
				// If the user's cached credentials are valid, the OptionalPendingResult will be "done"
				// and the GoogleSignInResult will be available instantly.
				Log.Debug(TAG, "Got cached sign-in");
				var result = opr.Get() as GoogleSignInResult;
				HandleSignInResult(result);
			}
			else
			{
				// If the user has not previously signed in on this device or the sign-in has expired,
				// this asynchronous branch will attempt to sign in the user silently.  Cross-device
				// single sign-on will occur in this branch.
				ShowProgressDialog();
				opr.SetResultCallback(new SignInResultCallback { Activity = this });
			}
		}

		protected override void OnResume()
		{
			base.OnResume();
			HideProgressDialog();
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			Log.Debug(TAG, "onActivityResult:" + requestCode + ":" + resultCode + ":" + data);

			// Result returned from launching the Intent from GoogleSignInApi.getSignInIntent(...);
			if (requestCode == RC_SIGN_IN)
			{
				var result = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
				HandleSignInResult(result);
			}
		}

		public void HandleSignInResult(GoogleSignInResult result)
		{
			Log.Debug(TAG, "handleSignInResult:" + result.IsSuccess);
			if (result.IsSuccess)
			{
				// Signed in successfully, show authenticated UI.
				var acct = result.SignInAccount;
				mStatusTextView.Text = string.Format(GetString(Resource.String.signed_in_fmt), acct.DisplayName);
				UpdateUI(true);
			}
			else
			{
				// Signed out, show unauthenticated UI.
				UpdateUI(false);
			}
		}

		void SignIn()
		{
			var signInIntent = Auth.GoogleSignInApi.GetSignInIntent(mGoogleApiClient);
			StartActivityForResult(signInIntent, RC_SIGN_IN);
		}

		void SignOut()
		{
			Auth.GoogleSignInApi.SignOut(mGoogleApiClient).SetResultCallback(new SignOutResultCallback { Activity = this });
		}

		void RevokeAccess()
		{
			Auth.GoogleSignInApi.RevokeAccess(mGoogleApiClient).SetResultCallback(new SignOutResultCallback { Activity = this });
		}

		public void OnConnectionFailed(ConnectionResult result)
		{
			// An unresolvable error has occurred and Google APIs (including Sign-In) will not
        	// be available.
			Log.Debug(TAG, "onConnectionFailed:" + result);
		}

		protected override void OnStop()
		{
			base.OnStop();
			mGoogleApiClient.Disconnect();
		}

		public void ShowProgressDialog()
		{
			if (mProgressDialog == null)
			{
				mProgressDialog = new ProgressDialog(this);
				mProgressDialog.SetMessage(GetString(Resource.String.loading));
				mProgressDialog.Indeterminate = true;
			}

			mProgressDialog.Show();
		}

		public void HideProgressDialog()
		{
			if (mProgressDialog != null && mProgressDialog.IsShowing)
			{
				mProgressDialog.Hide();
			}
		}

		public void UpdateUI (bool isSignedIn)
		{
			if (isSignedIn)
			{
				FindViewById(Resource.Id.sign_in_button).Visibility = ViewStates.Gone;
				FindViewById(Resource.Id.sign_out_and_disconnect).Visibility = ViewStates.Visible;
			}
			else
			{
				mStatusTextView.Text = GetString(Resource.String.signed_out);

				FindViewById(Resource.Id.sign_in_button).Visibility = ViewStates.Visible;
				FindViewById(Resource.Id.sign_out_and_disconnect).Visibility = ViewStates.Gone;
			}
		}

		public void OnClick(View v)
		{
			switch (v.Id)
			{
				case Resource.Id.sign_in_button:
					SignIn();
					break;
				case Resource.Id.sign_out_button:
					SignOut();
					break;
				case Resource.Id.disconnect_button:
					RevokeAccess();
					break;
			}
		}
	}
}


