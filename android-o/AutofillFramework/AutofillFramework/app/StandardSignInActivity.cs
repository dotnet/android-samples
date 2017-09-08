using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;

namespace AutofillFramework.app
{
	[Activity(Label = "StandardSignInActivity")]
	public class StandardSignInActivity : AppCompatActivity
	{
		EditText UsernameEditText;
		EditText PasswordEditText;

		public static Intent GetStartActivityIntent(Context context)
		{
			return new Intent(context, typeof(StandardSignInActivity));
    	}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.login_activity);
			UsernameEditText = (EditText)FindViewById(Resource.Id.usernameField);
			PasswordEditText = (EditText)FindViewById(Resource.Id.passwordField);

			FindViewById(Resource.Id.login).Click += (sender, e) => {
				Login();
			};
			FindViewById(Resource.Id.clear).Click += (sender, e) => {
				ResetFields();
			};
		}

		void ResetFields()
		{
			UsernameEditText.Text = "";
			PasswordEditText.Text = "";
		}

		/// <summary>
		/// Emulates a login action.
		/// </summary>
		void Login()
		{
			var username = UsernameEditText.Text;
			var password = PasswordEditText.Text;
			var valid = IsValidCredentials(username, password);
			if (valid)
			{
				StartActivity(WelcomeActivity.GetStartActivityIntent(this));
				Finish();
			}
			else
			{
				Toast.MakeText(this, "Authentication failed.", ToastLength.Short).Show();
			}
		}

		/// <summary>
		/// Dummy implementation for demo purposes. A real service should use secure mechanisms to
		/// authenticate users.
		/// </summary>
		/// <returns><c>true</c>, if valid credentials are valid, <c>false</c> otherwise.</returns>
		/// <param name="username">Username.</param>
		/// <param name="password">Password.</param>
		public bool IsValidCredentials(string username, string password)
		{
			return username != null && password != null && username.Equals(password);
		}
	}
}
