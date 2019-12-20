using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;

namespace AutofillFramework
{
    [Activity(Label = "StandardSignInActivity")]
    [Register("com.xamarin.AutofillFramework.StandardSignInActivity")]
    public class StandardSignInActivity : AppCompatActivity
    {
        private EditText mUsernameEditText;
        private EditText mPasswordEditText;

        public static Intent GetStartActivityIntent(Context context)
        {
            var intent = new Intent(context, typeof(StandardSignInActivity));
            return intent;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.login_activity);
            mUsernameEditText = FindViewById<EditText>(Resource.Id.usernameField);
            mPasswordEditText = FindViewById<EditText>(Resource.Id.passwordField);
            FindViewById(Resource.Id.login).Click += delegate { Login(); };

            FindViewById(Resource.Id.clear).Click += delegate { ResetFields(); };
        }

        private void ResetFields()
        {
            mUsernameEditText.Text = string.Empty;
            mPasswordEditText.Text = string.Empty;
        }

        /**
         * Emulates a login action.
         */
        private void Login()
        {
            var username = mUsernameEditText.Text;
            var password = mPasswordEditText.Text;
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