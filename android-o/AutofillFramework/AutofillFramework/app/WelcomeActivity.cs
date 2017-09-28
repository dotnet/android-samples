using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;

namespace AutofillFramework.app
{
	[Activity(Label = "WelcomeActivity")]
	public class WelcomeActivity : AppCompatActivity
	{
		public static Intent GetStartActivityIntent(Context context)
		{
			return new Intent(context, typeof(WelcomeActivity));
    	}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.welcome_activity);
			TextView countdownText = (TextView)FindViewById(Resource.Id.countdownText);
			var countDown = new CountdownTimerImpl(this, countdownText, 5000, 1000);
			countDown.Start();
		}
	}
}
