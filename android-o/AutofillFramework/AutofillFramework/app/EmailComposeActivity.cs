using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;

namespace AutofillFramework.app
{
	[Activity(Label = "EmailComposeActivity")]
	public class EmailComposeActivity : AppCompatActivity
	{
		public static Intent GetStartActivityIntent(Context context)
		{
			return new Intent(context, typeof(EmailComposeActivity));
    	}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.email_compose_activity);
			FindViewById(Resource.Id.sendButton).Click += (sender, args) => {
				StartActivity(WelcomeActivity.GetStartActivityIntent(this));
				Finish();
			};
		}
	}
}
