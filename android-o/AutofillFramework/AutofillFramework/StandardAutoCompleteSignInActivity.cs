using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.Autofill;
using Android.Widget;
using AndroidX.AppCompat.App;
using Java.Lang;

namespace AutofillFramework
{
    [Activity(Label = "StandardAutoCompleteSignInActivity")]
    [Register("com.xamarin.AutofillFramework.StandardAutoCompleteSignInActivity")]
    public class StandardAutoCompleteSignInActivity : AppCompatActivity
    {
        private AutoCompleteTextView mUsernameAutoCompleteField;
        private TextView mPasswordField;
        private TextView mLoginButton;
        private TextView mClearButton;
        private bool mAutofillReceived = false;
        private AutofillManager.AutofillCallback mAutofillCallback;
        private AutofillManager mAutofillManager;

        public static Intent GetStartActivityIntent(Context context)
        {
            var intent = new Intent(context, typeof(StandardAutoCompleteSignInActivity));
            return intent;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.login_with_autocomplete_activity);

            mLoginButton = FindViewById<TextView>(Resource.Id.login);
            mClearButton = FindViewById<TextView>(Resource.Id.clear);
            mUsernameAutoCompleteField = FindViewById<AutoCompleteTextView>(Resource.Id.usernameField);
            mPasswordField = FindViewById<TextView>(Resource.Id.passwordField);
            mLoginButton.Click += delegate { Login(); };

            mLoginButton.Click += delegate { ResetFields(); };

            mAutofillCallback = new MyAutofillCallback {that = this};
            mAutofillManager = (AutofillManager) GetSystemService(Class.FromType(typeof(AutofillManager)));

            var mockAutocompleteAdapter = ArrayAdapter.CreateFromResource(this,
                Resource.Array.mock_autocomplete_sign_in_suggestions,
                Android.Resource.Layout.SimpleDropDownItem1Line);
            mUsernameAutoCompleteField.Adapter = mockAutocompleteAdapter;
        }

        protected override void OnResume()
        {
            base.OnResume();
            mAutofillManager.RegisterCallback(mAutofillCallback);
        }

        protected override void OnPause()
        {
            base.OnPause();
            mAutofillManager.UnregisterCallback(mAutofillCallback);
        }

        private void ResetFields()
        {
            mUsernameAutoCompleteField.Text = string.Empty;
            mPasswordField.Text = string.Empty;
        }

        /**
         * Dummy implementation for demo purposes. A real service should use secure mechanisms to
         * authenticate users.
         */
        public bool IsValidCredentials(string username, string password)
        {
            return username != null && password != null && username.Equals(password);
        }

        /**
        * Emulates a login action.
        */
        private void Login()
        {
            var username = mUsernameAutoCompleteField.Text;
            var password = mPasswordField.Text;
            bool valid = IsValidCredentials(username, password);
            if (valid)
            {
                var intent = WelcomeActivity.GetStartActivityIntent(this);
                StartActivity(intent);
                Finish();
            }
            else
            {
                Toast.MakeText(this, "Authentication failed.", ToastLength.Short).Show();
            }
        }


        private class MyAutofillCallback : AutofillManager.AutofillCallback
        {
            public StandardAutoCompleteSignInActivity that;

            public override void OnAutofillEvent(View view, AutofillEventType e)
            {
                base.OnAutofillEvent(view, e);
                if (view is AutoCompleteTextView)
                {
                    switch (e)
                    {
                        case AutofillEventType.InputUnavailable:
                        // no break on purpose
                        case AutofillEventType.InputHidden:
                            if (!that.mAutofillReceived)
                            {
                                ((AutoCompleteTextView) view).ShowDropDown();
                            }
                            break;
                        case AutofillEventType.InputShown:
                            that.mAutofillReceived = true;
                            ((AutoCompleteTextView) view).Adapter = null;
                            break;
                        default:
                            Log.Debug(CommonUtil.TAG, "Unexpected callback: " + e);
                            break;
                    }
                }
            }
        }
    }
}