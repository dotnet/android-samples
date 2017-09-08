using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Views.Autofill;
using Android.Widget;

namespace AutofillFramework.app
{
	[Activity(Label = "StandardAutoCompleteSignInActivity")]
	public class StandardAutoCompleteSignInActivity : AppCompatActivity
	{
		AutoCompleteTextView UsernameAutoCompleteField;
		TextView PasswordField;
		TextView LoginButton;
		TextView ClearButton;
		AutofillManager.AutofillCallback AutofillCallback;
		AutofillManager AutofillManager;

		public static Intent GetStartActivityIntent(Context context)
		{
			return new Intent(context, typeof(StandardAutoCompleteSignInActivity));
    	}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.login_with_autocomplete_activity);

			LoginButton = (TextView)FindViewById(Resource.Id.login);
			ClearButton = (TextView)FindViewById(Resource.Id.clear);
			UsernameAutoCompleteField = (AutoCompleteTextView)FindViewById(Resource.Id.usernameField);
			PasswordField = (TextView)FindViewById(Resource.Id.passwordField);

			LoginButton.Click += (sender, args) => {
				Login();
			};
			ClearButton.Click += (sender, args) =>
			{
				ResetFields();
			};
			AutofillCallback = new MyAutofillCallback();
			AutofillManager = (AutofillManager) ApplicationContext.GetSystemService(Java.Lang.Class.FromType(typeof(AutofillManager)));
        	ArrayAdapter mockAutocompleteAdapter = ArrayAdapter.CreateFromResource(this, Resource.Array.mock_autocomplete_sign_in_suggestions,
			     Android.Resource.Layout.SimpleDropDownItem1Line);
			UsernameAutoCompleteField.Adapter = mockAutocompleteAdapter;
		}

		protected override void OnResume()
		{
			base.OnResume();
			AutofillManager.RegisterCallback(AutofillCallback);
		}

		protected override void OnPause()
		{
			base.OnPause();
			AutofillManager.UnregisterCallback(AutofillCallback);
		}

		void ResetFields()
		{
			UsernameAutoCompleteField.Text = "";
			PasswordField.Text = "";
		}

		/// <summary>
		/// Emulates a login action.
		/// </summary>
		void Login()
		{
			var username = UsernameAutoCompleteField.Text;
			var password = PasswordField.Text;
			bool valid = IsValidCredentials(username, password);
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
