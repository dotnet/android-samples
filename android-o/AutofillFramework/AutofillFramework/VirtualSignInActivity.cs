using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Views.Autofill;
using Android.Widget;
using Java.Lang;

namespace AutofillFramework
{
    [Activity(Label = "VirtualSignInActivity")]
    [Register("com.xamarin.AutofillFramework.VirtualSignInActivity")]
    public class VirtualSignInActivity : AppCompatActivity
    {
        private CustomVirtualView mCustomVirtualView;
        private AutofillManager mAutofillManager;
        private CustomVirtualView.Line mUsernameLine;
        private CustomVirtualView.Line mPasswordLine;

        public static Intent GetStartActivityIntent(Context context)
        {
            var intent = new Intent(context, typeof(VirtualSignInActivity));
            return intent;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.virtual_login_activity);

            mCustomVirtualView = FindViewById<CustomVirtualView>(Resource.Id.custom_view);

            var credentialsPartition =
                mCustomVirtualView.AddPartition(GetString(Resource.String.partition_credentials));
            mUsernameLine = credentialsPartition.AddLine("username", (int) AutofillType.Text,
                GetString(Resource.String.username_label),
                "         ", false, View.AutofillHintUsername);
            mPasswordLine = credentialsPartition.AddLine("password", (int) AutofillType.Text,
                GetString(Resource.String.password_label),
                "         ", true, View.AutofillHintPassword);

            FindViewById(Resource.Id.login).Click += delegate { Login(); };

            FindViewById(Resource.Id.clear).Click += delegate
            {
                ResetFields();
                mAutofillManager.Cancel();
            };
            mAutofillManager = (AutofillManager) GetSystemService(Class.FromType(typeof(AutofillManager)));
        }

        private void ResetFields()
        {
            mUsernameLine.Reset();
            mPasswordLine.Reset();
            mCustomVirtualView.PostInvalidate();
        }

        /**
         * Emulates a login action.
         */
        private void Login()
        {
            var username = mUsernameLine.GetText().ToString();
            var password = mPasswordLine.GetText().ToString();
            var valid = IsValidCredentials(username, password);
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

        /**
         * Dummy implementation for demo purposes. A real service should use secure mechanisms to
         * authenticate users.
         */
        public bool IsValidCredentials(string username, string password)
        {
            return username != null && password != null && username.Equals(password);
        }
    }
}