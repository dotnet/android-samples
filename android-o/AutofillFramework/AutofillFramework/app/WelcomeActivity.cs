
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace AutofillFramework.app
{
	[Activity(Label = "WelcomeActivity")]
	public class WelcomeActivity : AppCompatActivity
	{

		class CountDownTimerImpl : CountDownTimer
		{
			public TextView CountdownText { get; set; }
			public WelcomeActivity Activity { get; set; }

			public CountDownTimerImpl(WelcomeActivity activity, TextView countdownText, 
			    long millisInFuture, long countDownInterval) : base(millisInFuture, countDownInterval) 
			{
				Activity = activity;
				CountdownText = countdownText;
			}
			
			public override void OnFinish()
			{
				if (!Activity.IsFinishing) {
					Activity.Finish();
				}
			}

			public override void OnTick(long millisUntilFinished)
			{
				int secondsRemaining = Java.Lang.Math.ToIntExact(millisUntilFinished / 1000);
				CountdownText.Text = CountdownText.Context.Resources
					.GetQuantityString(Resource.Plurals.welcome_page_countdown, secondsRemaining, secondsRemaining);
			}
		}

		public static Intent GetStartActivityIntent(Context context)
		{
			return new Intent(context, typeof(WelcomeActivity));
    	}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.welcome_activity);
			TextView countdownText = (TextView)FindViewById(Resource.Id.countdownText);
			var countDown = new CountDownTimerImpl(this, countdownText, 5000, 1000);
			countDown.Start();
		}
	}
}
