using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Views.Autofill;
using Android.Widget;

namespace AutofillFramework.app
{
	[Activity(Label = "VirtualSignInActivity")]
	public class VirtualSignInActivity : AppCompatActivity
	{
		CustomVirtualView CustomVirtualView;
		AutofillManager AutofillManager;

		public static Intent GetStartActivityIntent(Context context)
		{
			return new Intent(context, typeof(VirtualSignInActivity));
    	}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.virtual_login_activity);
			CustomVirtualView = (CustomVirtualView)FindViewById(Resource.Id.custom_view);

			FindViewById(Resource.Id.login).Click += (sender, e) => {
				Login();
			};
			FindViewById(Resource.Id.clear).Click += (sender, e) => {
				ResetFields();
			};
			AutofillManager = (AutofillManager)ApplicationContext.GetSystemService(Java.Lang.Class.FromType(typeof(AutofillManager)));
		}

		void ResetFields()
		{
			CustomVirtualView.ResetFields();
		}


		/// <summary>
		///	Emulates a login action.  
		/// </summary>
		void Login()
		{
			var username = CustomVirtualView.UsernameText;
			var password = CustomVirtualView.PasswordText;
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
